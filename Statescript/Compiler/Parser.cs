﻿using System.Collections.Generic;
using Statescript.Compiler.AstNode;
using Statescript.Compiler.Tokens;

namespace Statescript.Compiler
{
   /// <summary>
   /// Parser analyzes a list of tokens produced by the tokenizer and builds a 
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
      /// Parses a sequence of tokens produced by the Tokenizer.
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
         int count = tokens.Count;
         Token t = tokens[_index];

         ParseBlanklines();

         ParseMachine();

         StateAstNode state;
         do {
            state = ParseState();
            if (state != null) {
               _machine.States.Add(state);
            }
         } while (state != null && !_exitEarly && _index < _tokens.Count);

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
               _machine.Name = GetDataSubstring(t);
            } else {
               HandleError("@machine missing name", t);
               return;
            }

            // build action for initial transition
            Advance();
            var action = ParseAction();
            if (action == null) {
               HandleError("@machine missing default transition", _tokens[_index]);
               return;
            }

            _machine.Action = action;
            Advance();
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

         state.Name = GetDataSubstring(t);
         Advance(); // move past identifier

         while (TryParseStateSection(state) && !_exitEarly && _index < _tokens.Count) {
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
            return true;
         }

         return false;
      }

      private ActionAstNode ParseAction()
      {
         Token t;
         Current(out t);
         // transition operator found. This is syntatic sugar. handle first.
         if (t.TokenType == TokenType.Operator && t.Operator == TokenOperator.Transition) {
            var action = new ActionAstNode
            {
               Name = ParserConstants.TransitionAction,
               LineNumber = t.LineNumber,
            };

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
            param.Val = GetDataSubstring(t);
            action.Params.Add(param);

            return action;
         }
         

         return null;
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
