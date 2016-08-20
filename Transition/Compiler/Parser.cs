using System.Collections.Generic;
using Transition.Compiler.AstNodes;
using Transition.Compiler.Tokens;

namespace Transition.Compiler
{
   /// <summary>
   /// Parser analyzes a list of tokens produced by the Scanner and builds a
   /// tree of AstNodes with a MachineAstNode as the root. It will also try to
   /// prouduce helpful errors.
   /// </summary>
   public class Parser
   {
      private MachineAstNode _machine;
      private bool _exitEarly;
      private int _index;
      private IList<Token> _tokens;
      private string _data;

      public Parser()
      {
      }

      /// <summary>
      /// Parses a sequence of tokens produced by the Scanner.
      /// </summary>
      /// <returns>
      /// A MachineAstNode that is ready for compilation
      /// </returns>
      /// <param name="tokens">Tokens.</param>
      public MachineAstNode Parse(IList<Token> tokens, string data)
      {
         _machine = null;
         _index = 0;
         _exitEarly = false;
         _data = data;
         _tokens = tokens;

         ParseBlanklines();

         ParseMachine();

         StateAstNode state;
         do {
            state = ParseState();
            if (state != null) {
               _machine.States.Add(state);
            }
         } while (state != null && ShouldContinueParsing());

         _tokens = null;
         _data = null;
         return _machine;
      }

      private void ParseBlanklines()
      {
         Token t;
         var tokenAvailable = Current(out t);
         while (tokenAvailable && t.TokenType == TokenType.NewLine) {
            tokenAvailable = Next(out t);
         }
      }

      private void ParseMachine()
      {
         var t = _tokens[_index];
         if (t.TokenType == TokenType.Keyword && t.Keyword == TokenKeyword.Machine) {
            _machine = new MachineAstNode();

            Next(out t);
            if (t.TokenType == TokenType.Identifier) {
               _machine.Identifier = GetDataSubstring(t);
            } else {
               HandleError("@machine missing name", t);
               return;
            }

            // build action for initial transition
            Advance();
            var action = ParseAction(false);
            if (action == null) {
               HandleError("@machine missing default transition", _tokens[_index]);
               return;
            }

            _machine.Action = action;
         }
      }

      private StateAstNode ParseState()
      {
         ParseBlanklines();
         Token t;
         if (!Current(out t)) {
            return null;
         }

         if (t.TokenType != TokenType.Keyword || t.Keyword != TokenKeyword.State) {
            HandleError("Expected @state but found ", t);
            return null;
         }

         var state = new StateAstNode
         {
            LineNumber = t.LineNumber,
         };

         if (!Next(out t)) {
            HandleError("Expected @state identifier, reached end of input.", t);
            return null;
         } else if (t.TokenType != TokenType.Identifier) {
            HandleError("Expected @state identifier found ", t);
            return null;
         }

         state.Identifier = GetDataSubstring(t);
         Advance(); // move past identifier

         while (TryParseStateSection(state) && ShouldContinueParsing()) {
            // loop until no more sections
         }

         return state;
      }

      private bool TryParseStateSection(StateAstNode state)
      {
         ParseBlanklines();
         Token t;
         if (!Current(out t)) {
            return false;
         } else if (t.TokenType != TokenType.Keyword) {
            HandleError("Expected keyword but found", t);
            return false;
         } else if (t.Keyword == TokenKeyword.Enter || t.Keyword == TokenKeyword.Exit ||
                    t.Keyword == TokenKeyword.Run || t.Keyword == TokenKeyword.On) {
            var section = new SectionAstNode
            {
               LineNumber = t.LineNumber,
            };
            switch (t.Keyword) {
               case TokenKeyword.Enter:
                  state.Enter = section;
                  break;
               case TokenKeyword.Run:
                  state.Run = section;
                  break;
               case TokenKeyword.Exit:
                  state.Exit = section;
                  break;
               case TokenKeyword.On:
                  state.On = section;
                  break;
            }
            // move past keyword
            Advance();
            ActionAstNode actionNode;
            while (ShouldContinueParsing()) {
               actionNode = ParseAction(t.Keyword == TokenKeyword.On);
               if (actionNode == null)
                  break;
               section.Actions.Add(actionNode);
            }
            return true;
         }

         return false;
      }

      private ActionAstNode ParseAction(bool lookForMessage)
      {
         ParseBlanklines();
         Token t;
         if (!Current(out t)) {
            return null;
         }

         // if a keyword is found we must be in a new section or state
         if (t.TokenType == TokenType.Keyword) {
            return null;
         }

         // transition operator found. This is syntatic sugar. handle first.
         if (t.TokenType == TokenType.Operator && t.Operator == TokenOperator.Transition) {
            return ParseTransitionAction(null);
         } else {

            ActionAstNode action = null;

            if (lookForMessage) {
               if (t.TokenType != TokenType.Value) {
                  HandleError("Action missing message. Found ", t);
                  return null;
               }

               action = new ActionAstNode
               {
                  LineNumber = t.LineNumber,
                  Message = GetDataSubstring(t)
               };
               // advance past assign operator after message
               if (!Advance()) {
                  HandleError("Unexpected end of input in action. Expected [:]", t);
                  return action;
               }
               // advance to identifier
               if (!Next(out t)) {
                  HandleError("Unexpected end of input in action. Expected action identifier", t);
                  return action;
               }
            }

            if (t.TokenType == TokenType.Operator && t.Operator == TokenOperator.Transition) {
               return ParseTransitionAction(action);
            } else if (t.TokenType == TokenType.Identifier) {
               if (action == null) {
                  action = new ActionAstNode
                  {
                     LineNumber = t.LineNumber,
                  };
               }
               action.Identifier = GetDataSubstring(t);

               // advance to params
               Advance();

               ParamAstNode paramNode;
               while (ShouldContinueParsing()) {
                  paramNode = ParseParam();
                  if (paramNode == null)
                     break;
                  action.Params.Add(paramNode);
               }

               return action;
            }
         }

         return null;
      }

      private ActionAstNode ParseTransitionAction(ActionAstNode action)
      {  
         Token t;
         if (!Current(out t)) {
            return null;
         }

         if (action == null) {
            action = new ActionAstNode();
         }

         action.Identifier = ParserConstants.TransitionAction;
         action.LineNumber = t.LineNumber;

         var param = new ParamAstNode
         {
            LineNumber = t.LineNumber,
            Op = ParamOperation.Transition
         };
         // aquire the value
         Next(out t);
         if (t.TokenType != TokenType.Value) {
            HandleError("transition missing value", _tokens[_index - 1]);
            return null;
         }
         param.Identifier = ParserConstants.DefaultParameterIdentifier;
         param.Val = GetDataSubstring(t);
         action.Params.Add(param);

         Advance();
         return action;
      }

      private ParamAstNode ParseParam()
      {
         Token t;
         // look for identifier
         if (!Current(out t)) {
            return null;
         } else if (t.TokenType == TokenType.NewLine) {
            return null;
         } else if (t.TokenType == TokenType.Operator && t.Operator == TokenOperator.Transition) {
            // Syntatic sugar: found a transition operator without an identifier, must be a default parameter
            var defaultParam = new ParamAstNode
            {
               LineNumber = t.LineNumber,
               Identifier = ParserConstants.DefaultParameterIdentifier,
               Op = ParamOperation.Transition
            };

            if (!Next(out t)) {
               HandleError("Expected a transition value. Reached end of input.", _tokens[_index - 1]);
               return defaultParam;
            } else if (t.TokenType != TokenType.Value) {
               HandleError("Expected a transition value but found", t);
               return defaultParam;
            }

            defaultParam.Val = GetDataSubstring(t);

            // move off val token
            Advance();
            return defaultParam;
         } else if (t.TokenType == TokenType.Value) {
            // Syntatic sugar: found a value without an identifier, must be a default parameter
            var defaultParam = new ParamAstNode
            {
               LineNumber = t.LineNumber,
               Identifier = ParserConstants.DefaultParameterIdentifier,
               Op = ParamOperation.Assign,
               Val = GetDataSubstring(t)
            };

            // move off val token
            Advance();
            return defaultParam;
         } else if (t.TokenType != TokenType.Identifier) {
            HandleError("Parameter missing identifier. Found ", t);
            return null;
         }

         var param = new ParamAstNode
         {
            LineNumber = t.LineNumber,
            Identifier = GetDataSubstring(t)
         };

         // look for operator
         if (!Next(out t)) {
            HandleError("Parameter missing operator and value. Reached end of input.", _tokens[_index - 1]);
            return param;
         } else if (t.TokenType != TokenType.Operator) {
            HandleError("Parameter missing operator and value. Found ", t);
            return param;
         }

         param.Op = t.Operator == TokenOperator.Assign ? ParamOperation.Assign : ParamOperation.Transition;

         // look for value
         if (!Next(out t)) {
            HandleError("Parameter missing value. Reached end of input.", _tokens[_index - 1]);
            return param;
         } else if (t.TokenType != TokenType.Value) {
            HandleError("Parameter missing value. Found ", t);
            return param;
         }

         param.Val = GetDataSubstring(t);

         // move off val token
         Advance();

         return param;
      }

      private bool ShouldContinueParsing()
      {
         return !(_exitEarly || _index >= _tokens.Count);
      }

      private string GetDataSubstring(Token t)
      {
         return _data.Substring(t.StartIndex, t.Length);
      }

      private bool Current(out Token t)
      {
         if (_index >= _tokens.Count) {
            t = new Token();
            return false;
         }

         t = _tokens[_index];
         return true;
      }

      private bool Next(out Token t)
      {
         _index++;
         if (_index >= _tokens.Count) {
            t = new Token();
            return false;
         }

         t = _tokens[_index];
         return true;
      }

      private bool Advance()
      {
         _index++;
         return _index < _tokens.Count;
      }

      private void HandleError(string message, Token error)
      {
         _exitEarly = true;
      }
   }
}
