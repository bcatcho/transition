using System;
using NUnit.Framework;
using Statescript.Compiler;
using System.Collections.Generic;

namespace Tests.Compiler
{
   [TestFixture]
   public class ParserTests
   {
      private Token KeywordToken(TokenKeyword keyword, int lineNumber)
      {
         return new Token {
            Keyword = keyword,
            TokenType = TokenType.Keyword,
            LineNumber = lineNumber
         };
      }

      private Token IdentifierToken(int start, int length, int lineNumber)
      {
         return new Token {
            StartIndex = start,
            Length = length,
            TokenType = TokenType.Identifier,
            LineNumber = lineNumber
         };
      }

      [Test]
      public void Parse_MachineLine_MachineHasName()
      {
         var input = "@machine machinename";
         var tokens = new List<Token> {
            KeywordToken(TokenKeyword.Machine, 1),
            IdentifierToken(9, "machinename".Length, 1)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);

         Assert.AreEqual(ast.Name, "machinename");
      }


      [Test]
      public void Parse_NewLinesThenMachineLine_MachineHasName()
      {
         var input = "\n\n\r\n@machine machinename";
         var tokens = new List<Token> {
            KeywordToken(TokenKeyword.Machine, 1),
            IdentifierToken(13, "machinename".Length, 1)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);

         Assert.AreEqual(ast.Name, "machinename");
      }
   }
}

