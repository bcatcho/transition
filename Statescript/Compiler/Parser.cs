using System.Collections.Generic;
using Statescript.Compiler.AstNode;

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
               _exitEarly = true;
               return;
            }
         }
      }

      private void HandleError(string message, Token error)
      {
      }
   }
}
