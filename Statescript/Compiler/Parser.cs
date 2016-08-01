using System;
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
      public MachineAstNode Parse(IList<Token> tokens) {
         return null;
      }
   }
}
