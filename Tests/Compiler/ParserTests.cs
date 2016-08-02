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
      private Token NLTkn(int lineNumber)
      {
         return new Token
         {
            TokenType = TokenType.NewLine,
            LineNumber = lineNumber
         };
      }

      private Token KeyTkn(TokenKeyword keyword, int lineNumber)
      {
         return new Token
         {
            Keyword = keyword,
            TokenType = TokenType.Keyword,
            LineNumber = lineNumber
         };
      }

      private Token OpTkn(TokenOperator op, int lineNumber)
      {
         return new Token
         {
            Operator = op,
            TokenType = TokenType.Operator,
            LineNumber = lineNumber
         };
      }

      private Token ValTkn(int start, int length, int lineNumber)
      {
         return new Token
         {
            StartIndex = start,
            Length = length,
            TokenType = TokenType.Value,
            LineNumber = lineNumber
         };
      }

      private Token IdTkn(int start, int length, int lineNumber)
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
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(9, 11, 1), OpTkn(TokenOperator.Transition, 1), ValTkn(26, 4, 1)
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
            NLTkn(1),
            NLTkn(1),
            NLTkn(1),
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 11, 1), OpTkn(TokenOperator.Transition, 1), ValTkn(30, 4, 1)
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
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 4, 1), OpTkn(TokenOperator.Transition, 1), ValTkn(18, 6, 1)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var param = ast.Action.Params[0];

         Assert.AreEqual(ParamOperation.Transition, param.Op);
         Assert.AreEqual("State1", param.Val);
      }

      [Test]
      public void Parse_OneState_OneStateMade()
      {
         var input = "@machine m -> 's'\n@state state1";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), ValTkn(19, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 1)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var state = ast.States[0];

         Assert.AreEqual("state1", state.Name);
      }

      [Test]
      public void Parse_TwoStatesNoSections_TwoStatesMade()
      {
         var input = "@machine m -> 's'\n@state state1\n@state state2";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), ValTkn(19, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 2), NLTkn(2),
            KeyTkn(TokenKeyword.State, 3), IdTkn(39, 6, 3)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);

         Assert.AreEqual("state1", ast.States[0].Name);
         Assert.AreEqual("state2", ast.States[1].Name);
      }

      [Test]
      [TestCase(TokenKeyword.On)]
      [TestCase(TokenKeyword.Enter)]
      [TestCase(TokenKeyword.Exit)]
      [TestCase(TokenKeyword.Run)]
      public void Parse_StateWithOneSection_StateHasSection(TokenKeyword sectionKeyword)
      {
         // the "section" doesn't matter as the input is not used in the asserts
         var input = "@machine m -> 's'\n@state state1\n\tsection";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), ValTkn(19, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 2), NLTkn(2),
            KeyTkn(sectionKeyword, 3)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var state = ast.States[0];

         if (sectionKeyword == TokenKeyword.Enter)
            Assert.NotNull(state.Enter);
         else if (sectionKeyword == TokenKeyword.On)
            Assert.NotNull(state.On);
         else if (sectionKeyword == TokenKeyword.Exit)
            Assert.NotNull(state.Exit);
         else if (sectionKeyword == TokenKeyword.Run)
            Assert.NotNull(state.Run);
      }

      [Test]
      public void Parse_StateSectionWithOneAction_StateSectionHasOneAction()
      {
         var input = "@machine m -> 's'\n@state state1\n\trun\n\tdoThing";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), ValTkn(19, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 2), NLTkn(2),
            KeyTkn(TokenKeyword.Run, 3), NLTkn(3),
            IdTkn(38, 7, 4)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var state = ast.States[0];

         Assert.AreEqual("doThing", state.Run.Actions[0].Name);
      }
   }
}

