using System;
using NUnit.Framework;
using Statescript.Compiler;
using System.Collections.Generic;
using Statescript.Compiler.AstNode;
using Statescript.Compiler.Tokens;

namespace Tests.Compiler
{
   [TestFixture]
   public class ParserTests
   {
      private Token KeywordToken(TokenKeyword keyword, int lineNumber)
      {
         return new Token
         {
            Keyword = keyword,
            TokenType = TokenType.Keyword,
            LineNumber = lineNumber
         };
      }

      private Token OpToken(TokenOperator op, int lineNumber)
      {
         return new Token
         {
            Operator = op,
            TokenType = TokenType.Operator,
            LineNumber = lineNumber
         };
      }

      private Token ValToken(int start, int length, int lineNumber)
      {
         return new Token
         {
            StartIndex = start,
            Length = length,
            TokenType = TokenType.Value,
            LineNumber = lineNumber
         };
      }

      private Token IdentifierToken(int start, int length, int lineNumber)
      {
         return new Token
         {
            StartIndex = start,
            Length = length,
            TokenType = TokenType.Identifier,
            LineNumber = lineNumber
         };
      }

      [Test]
      public void Parse_MachineLine_MachineHasName()
      {
         var input = "@machine machinename -> 'blah'";
         var tokens = new List<Token>
         {
            KeywordToken(TokenKeyword.Machine, 1),
            IdentifierToken(9, "machinename".Length, 1),
            OpToken(TokenOperator.Transition, 1),
            ValToken(26, 4, 1)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);

         Assert.AreEqual(ast.Name, "machinename");
      }


      [Test]
      public void Parse_NewLinesThenMachineLine_MachineHasName()
      {
         var input = "\n\n\r\n@machine machinename -> 'blah'";
         var tokens = new List<Token>
         {
            KeywordToken(TokenKeyword.Machine, 1),
            IdentifierToken(13, "machinename".Length, 1),
            OpToken(TokenOperator.Transition, 1),
            ValToken(30, 4, 1)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);

         Assert.AreEqual("machinename", ast.Name);
      }

      [Test]
      public void Parse_NewLinesThenMachineLine_HasTransition()
      {
         var input = "@machine mach -> 'State1'";
         var tokens = new List<Token>
         {
            KeywordToken(TokenKeyword.Machine, 1),
            IdentifierToken(13, "machinename".Length, 1),
            OpToken(TokenOperator.Transition, 1),
            ValToken(18, 6, 1)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var param = ast.Action.Params[0];

         Assert.AreEqual(ParamOperation.Transition, param.Op);
         Assert.AreEqual("State1", param.Val);
      }
   }
}

