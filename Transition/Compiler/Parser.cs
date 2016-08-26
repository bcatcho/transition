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
      private int _index;
      private IList<Token> _tokens;
      private string _data;
      private Token[] _t;

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
         _data = data;
         _tokens = tokens;
         _t = new Token[3];

         Advance(0);

         ParseMachine();

         StateAstNode state;
         do {
            state = ParseState();
            if (state != null) {
               _machine.States.Add(state);
            }
         } while (state != null);

         _tokens = null;
         _data = null;
         return _machine;
      }

      private bool Exists(int i)
      {
         return _index + i < _tokens.Count;
      }

      private void Advance()
      {
         Advance(1);
      }

      private void Advance(int count)
      {
         _index += count;
         for (int i = 0; i < 3 && _index + i < _tokens.Count; ++i) {
            _t[i] = _tokens[_index + i];
         }
      }

      private void AdvanceNewLine()
      {
         if (Exists(0) && _t[0].TokenType == TokenType.NewLine) {
            Advance();
         }
      }

      private void ParseMachine()
      {
         AdvanceNewLine();
         if (_t[0].TokenType == TokenType.Keyword && _t[0].Keyword == TokenKeyword.Machine) {
            _machine = new MachineAstNode();


            if (Exists(1) && _t[1].TokenType == TokenType.Identifier) {
               _machine.Identifier = GetDataSubstring(_t[1]);
            } else {
               HandleError("@machine missing name", _t[1]);
               return;
            }

            // build action for initial transition
            Advance(2);
            var action = ParseAction(false);
            if (action == null) {
               HandleError("@machine missing default transition", _t[0]);
               return;
            }

            _machine.Action = action;

            // token 0 should be a newline
            if (Exists(1) && _t[1].TokenType == TokenType.Keyword && _t[1].Keyword == TokenKeyword.On) {
               _machine.On = new SectionAstNode();
               // move past @on keyword
               Advance(2);
               ActionAstNode actionNode;
               while (Exists(0)) {
                  actionNode = ParseAction(true);
                  if (actionNode == null)
                     break;
                  _machine.On.Actions.Add(actionNode);
               }
            }
         }
      }

      private StateAstNode ParseState()
      {
         AdvanceNewLine();
         if (!Exists(0)) {
            return null;
         }

         if (_t[0].TokenType != TokenType.Keyword || _t[0].Keyword != TokenKeyword.State) {
            HandleError("Expected @state.", _t[0]);
            return null;
         }

         var state = new StateAstNode
         {
            LineNumber = _t[0].LineNumber,
         };

         if (!Exists(1)) {
            HandleError("Expected @state identifier, reached end of input.", _t[0]);
            return null;
         } else if (_t[1].TokenType != TokenType.Identifier) {
            HandleError("Expected @state identifier.", _t[1]);
            return null;
         }

         state.Identifier = GetDataSubstring(_t[1]);
         Advance(2); // move past identifier

         while (TryParseStateSection(state) && Exists(0)) {
            // loop until no more sections
         }

         return state;
      }

      private bool TryParseStateSection(StateAstNode state)
      {
         AdvanceNewLine();
         if (!Exists(0)) {
            return false;
         } else if (_t[0].TokenType != TokenType.Keyword) {
            HandleError("Expected keyword.", _t[0]);
            return false;
         } else if (_t[0].Keyword == TokenKeyword.Enter || _t[0].Keyword == TokenKeyword.Exit ||
                    _t[0].Keyword == TokenKeyword.Run || _t[0].Keyword == TokenKeyword.On) {
            var section = new SectionAstNode
            {
               LineNumber = _t[0].LineNumber,
            };
            switch (_t[0].Keyword) {
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
            var isOnSection = _t[0].Keyword == TokenKeyword.On;
            Advance();
            ActionAstNode actionNode;
            while (Exists(0)) {
               actionNode = ParseAction(isOnSection);
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
         AdvanceNewLine();
         if (!Exists(0)) {
            return null;
         }

         // if a keyword is found we must be in a new section or state
         if (_t[0].TokenType == TokenType.Keyword) {
            return null;
         }

         // transition operator found. This is syntatic sugar. handle first.
         if (_t[0].TokenType == TokenType.Operator && _t[0].Operator == TokenOperator.Transition) {
            return ParseTransitionAction(null);
         } else {

            ActionAstNode action = null;

            if (lookForMessage) {
               if (_t[0].TokenType != TokenType.Value) {
                  HandleError("Action missing message.", _t[0]);
                  return null;
               }

               action = new ActionAstNode
               {
                  LineNumber = _t[0].LineNumber,
                  Message = GetDataSubstring(_t[0])
               };
               // advance past assign operator after message
               if (!Exists(1)) {
                  HandleError("Unexpected end of input in action. Expected [:]", _t[1]);
                  return action;
               }
               // advance to identifier
               if (!Exists(2)) {
                  HandleError("Unexpected end of input in action. Expected action identifier", _t[2]);
                  return action;
               }
               // make the current token the start of the action
               Advance(2);
            }

            if (_t[0].TokenType == TokenType.Operator && _t[0].Operator == TokenOperator.Transition) {
               return ParseTransitionAction(action);
            } else if (_t[0].TokenType == TokenType.Identifier) {
               if (action == null) {
                  action = new ActionAstNode
                  {
                     LineNumber = _t[0].LineNumber,
                  };
               }
               action.Identifier = GetDataSubstring(_t[0]);

               // advance to params
               Advance();

               ParamAstNode paramNode;
               while (Exists(0)) {
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
         if (!Exists(0)) {
            return null;
         }

         if (action == null) {
            action = new ActionAstNode();
         }

         action.Identifier = ParserConstants.TransitionAction;
         action.LineNumber = _t[0].LineNumber;

         var param = new ParamAstNode
         {
            LineNumber = _t[0].LineNumber,
            Op = ParamOperation.Transition
         };
         // aquire the value
         if (!Exists(1)) {
            HandleError("Expected a transition value. Reached end of input.", _t[0]);
            return null;
         } 
         if (_t[1].TokenType != TokenType.Identifier) {
            HandleError("transition missing value", _tokens[1]);
            return null;
         }
         param.Identifier = ParserConstants.DefaultParameterIdentifier;
         param.Val = GetDataSubstring(_t[1]);
         action.Params.Add(param);

         Advance(2);
         return action;
      }

      private ParamAstNode ParseParam()
      {
         // look for identifier
         if (!Exists(0)) {
            return null;
         } else if (_t[0].TokenType == TokenType.NewLine) {
            return null;
         } else if (_t[0].TokenType == TokenType.Operator && _t[0].Operator == TokenOperator.Transition) {
            // Syntatic sugar: found a transition operator without an identifier, must be a default parameter
            var defaultParam = new ParamAstNode
            {
               LineNumber = _t[0].LineNumber,
               Identifier = ParserConstants.DefaultParameterIdentifier,
               Op = ParamOperation.Transition
            };

            if (!Exists(1)) {
               HandleError("Expected a transition value. Reached end of input.", _t[0]);
               return defaultParam;
            } else if (_t[1].TokenType != TokenType.Identifier) {
               HandleError("Expected a transition destination.", _t[1]);
               return defaultParam;
            }

            defaultParam.Val = GetDataSubstring(_t[1]);

            // move off identifier token
            Advance(2);
            return defaultParam;
         } else if (_t[0].TokenType == TokenType.Value) {
            // Syntatic sugar: found a value without an identifier, must be a default parameter
            var defaultParam = new ParamAstNode
            {
               LineNumber = _t[0].LineNumber,
               Identifier = ParserConstants.DefaultParameterIdentifier,
               Op = ParamOperation.Assign,
               Val = GetDataSubstring(_t[0])
            };

            // move off val token
            Advance();
            return defaultParam;
         } else if (_t[0].TokenType != TokenType.Identifier) {
            HandleError("Parameter missing identifier.", _t[0]);
            return null;
         }

         var param = new ParamAstNode
         {
            LineNumber = _t[0].LineNumber,
            Identifier = GetDataSubstring(_t[0])
         };

         // look for operator
         if (!Exists(1)) {
            HandleError("Parameter missing operator and value. Reached end of input.", _t[0]);
            return param;
         } else if (_t[1].TokenType != TokenType.Operator) {
            HandleError("Parameter missing operator and value.", _t[1]);
            return param;
         }

         param.Op = _t[1].Operator == TokenOperator.Assign ? ParamOperation.Assign : ParamOperation.Transition;

         // look for value
         if (!Exists(2)) {
            HandleError("Parameter missing value. Reached end of input.", _t[1]);
            return param;
         } else if (param.Op == ParamOperation.Assign && _t[2].TokenType != TokenType.Value) {
            HandleError("Parameter missing value.", _t[2]);
            return param;
         } else if (param.Op == ParamOperation.Transition && _t[2].TokenType != TokenType.Identifier) {
            HandleError("Parameter missing transition destination.", _t[2]);
            return param;
         }

         param.Val = GetDataSubstring(_t[2]);

         // move off val token
         Advance(3);

         return param;
      }

      private string GetDataSubstring(Token t)
      {
         return _data.Substring(t.StartIndex, t.Length);
      }

      private void HandleError(string message, Token error)
      {
         // in the future errors should be handled more gracefully
         throw new System.Exception(string.Format("[Parsing error on Line {0}] {1}", error.LineNumber, message));
      }
   }
}
