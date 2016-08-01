using System.Collections.Generic;
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
         int count = tokens.Count;
         Token t = tokens[_index];

         while (_index < count && t.TokenType == TokenType.NewLine) {
            _index++;
         } 

         LookForMachine(tokens, data);

         return _machine;
      }

      private void LookForMachine(IList<Token> tokens, string data)
      {
         var t = tokens[_index];
         if (t.TokenType == TokenType.Keyword && t.Keyword == TokenKeyword.Machine) {
            _machine = new MachineAstNode();

            _index++;
            t = tokens[_index];
            if (t.TokenType == TokenType.Identifier) {
               _machine.Name = data.Substring(t.StartIndex, t.Length);
            } else {
               HandleError("@machine missing name", t);
               return;
            }

            // build action for initial transition
            _index++;
            var action = BuildAction(tokens, data);
            if (action == null) {
               HandleError("@machine missing default transition", tokens[_index]);
               return;
            }

            _machine.Action = action;
         }
      }

      private ActionAstNode BuildAction(IList<Token> tokens, string data)
      {
         var t = tokens[_index];
         // transition operator found. This is syntatic sugar. handle first.
         if (t.TokenType == TokenType.Operator && t.Operator == TokenOperator.Transition) {
            var action = new ActionAstNode
            {
               Name = ParserConstants.TransitionAction,
               LineNumber = t.LineNumber,
            };

            var param = new ParamAstNode {
               LineNumber = t.LineNumber,
               Op = ParamOperation.Transition
            };
            // aquire the value
            _index++;
            t = tokens[_index];
            if (t.TokenType != TokenType.Value) {
               HandleError("transition missing value", tokens[_index - 1]);
               return null;
            }
            param.Val = data.Substring(t.StartIndex, t.Length);
            action.Params.Add(param);

            return action;
         }
         

         return null;
      }

      private void HandleError(string message, Token error)
      {
         _exitEarly = true;
      }
   }
}
