using System;
using NUnit.Framework;
using Transition.Compiler;
using System.Collections.Generic;
using Transition.Compiler.AstNodes;
using Transition.Compiler.Tokens;

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
         var input = "@machine machinename -> blah";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(9, 11, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(24, 4, 1)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);

         Assert.AreEqual(ast.Name, "machinename");
      }


      [Test]
      public void Parse_NewLinesThenMachineLine_MachineHasName()
      {
         var input = "\n\n\r\n@machine machinename -> blah";
         var tokens = new List<Token>
         {
            NLTkn(1),
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 11, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(28, 4, 1)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);

         Assert.AreEqual("machinename", ast.Name);
      }

      [Test]
      public void Parse_NewLinesThenMachineLine_HasTransition()
      {
         var input = "@machine mach -> State1";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 4, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(17, 6, 1)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var param = ast.Action.Params[0];

         Assert.AreEqual(ParamOperation.Transition, param.Op);
         Assert.AreEqual("State1", param.Val);
      }

      [Test]
      public void Parse_MachineWithMessages_HasMessages()
      {
         var input = "@machine mach -> State1\n@on\n'msg': dothing";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 4, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(17, 6, 1), NLTkn(1),
            KeyTkn(TokenKeyword.On, 2), NLTkn(2),
            ValTkn(29,3,3), OpTkn(TokenOperator.Assign,3), IdTkn(35,7,3)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var action = ast.On.Actions[0];

         Assert.AreEqual("dothing", action.Name);
         Assert.AreEqual("msg", action.Message);
      }

      [Test]
      public void Parse_OneState_OneStateMade()
      {
         var input = "@machine m -> s\n@state state1";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(18, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(23, 6, 1)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var state = ast.States[0];

         Assert.AreEqual("state1", state.Name);
      }

      [Test]
      public void Parse_TwoStatesNoSections_TwoStatesMade()
      {
         var input = "@machine m -> s\n@state state1\n@state state2";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(18, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(23, 6, 2), NLTkn(2),
            KeyTkn(TokenKeyword.State, 3), IdTkn(37, 6, 3)
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
         var input = "@machine m -> s  \n@state state1\n\tsection";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(18, 1, 1), NLTkn(1),
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
      public void Parse_OneAction_HasOneAction()
      {
         var input = "@machine m -> s  \n@state state1\n\trun\n\tact1";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(18, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 2), NLTkn(2),
            KeyTkn(TokenKeyword.Run, 3), NLTkn(3),
            IdTkn(38, 4, 4)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var state = ast.States[0];

         Assert.AreEqual("act1", state.Run.Actions[0].Name);
      }

      [Test]
      public void Parse_TwoActions_HasTwoActionsInCorrectOrder()
      {
         var input = "@machine m -> s  \n@state state1\n\trun\n\tact1\n\tact2";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(18, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 2), NLTkn(2),
            KeyTkn(TokenKeyword.Run, 3), NLTkn(3),
            IdTkn(38, 4, 4), NLTkn(4),
            IdTkn(44, 4, 5)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var state = ast.States[0];

         Assert.AreEqual("act1", state.Run.Actions[0].Name);
         Assert.AreEqual("act2", state.Run.Actions[1].Name);
      }

      [Test]
      public void Parse_ActionWithAssignParam_ActionHasAssignParam()
      {
         var input = "@machine m -> s  \n@state state1\n\trun\n\tact1 param:'val'";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(18, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 2), NLTkn(2),
            KeyTkn(TokenKeyword.Run, 3), NLTkn(3),
            IdTkn(38, 4, 4), IdTkn(43, 5, 4), OpTkn(TokenOperator.Assign, 5), ValTkn(50, 3, 5)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var param = ast.States[0].Run.Actions[0].Params[0];

         Assert.AreEqual("param", param.Name);
         Assert.AreEqual(ParamOperation.Assign, param.Op);
         Assert.AreEqual("val", param.Val);
      }

      [Test]
      public void Parse_ActionWithDefaultParam_ActionHasParam()
      {
         var input = "@machine m -> s  \n@state state1\n\trun\n\tact1 'val'";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(18, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 2), NLTkn(2),
            KeyTkn(TokenKeyword.Run, 3), NLTkn(3),
            IdTkn(38, 4, 4), ValTkn(44, 3, 5)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var param = ast.States[0].Run.Actions[0].Params[0];

         Assert.AreEqual(ParserConstants.DefaultParameterName, param.Name);
         Assert.AreEqual(ParamOperation.Assign, param.Op);
         Assert.AreEqual("val", param.Val);
      }

      [Test]
      public void Parse_ActionWithDefaultTransitionParam_ActionHasParam()
      {
         var input = "@machine m -> s  \n@state state1\n\trun\n\tact1 -> val";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(18, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 2), NLTkn(2),
            KeyTkn(TokenKeyword.Run, 3), NLTkn(3),
            IdTkn(38, 4, 4), OpTkn(TokenOperator.Transition, 4), IdTkn(46, 3, 5)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var param = ast.States[0].Run.Actions[0].Params[0];

         Assert.AreEqual(ParserConstants.DefaultParameterName, param.Name);
         Assert.AreEqual(ParamOperation.Transition, param.Op);
         Assert.AreEqual("val", param.Val);
      }

      [Test]
      public void Parse_ActionWithTransitionParam_ActionHasTransitionParam()
      {
         var input = "@machine m -> s  \n@state state1\n\t@run\nact1 param-> val";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(19, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 2), NLTkn(2),
            KeyTkn(TokenKeyword.Run, 3), NLTkn(3),
            IdTkn(38, 4, 4), IdTkn(43, 5, 4), OpTkn(TokenOperator.Transition, 5), IdTkn(51, 3, 5)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var param = ast.States[0].Run.Actions[0].Params[0];

         Assert.AreEqual("param", param.Name);
         Assert.AreEqual(ParamOperation.Transition, param.Op);
         Assert.AreEqual("val", param.Val);
      }

      [Test]
      public void Parse_ActionWithTwoAssignParams_ActionHasTwoAssignParams()
      {
         var input = "@machine m -> s  \n@state state1\n\trun\n\tact1 param:'val' p2:'v2'";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(18, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 2), NLTkn(2),
            KeyTkn(TokenKeyword.Run, 3), NLTkn(3),
            IdTkn(38, 4, 4), IdTkn(43, 5, 4), OpTkn(TokenOperator.Assign, 5), ValTkn(50, 3, 5), IdTkn(55, 2, 4), OpTkn(TokenOperator.Assign, 5), ValTkn(59, 2, 5)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var param = ast.States[0].Run.Actions[0].Params[1];

         Assert.AreEqual("p2", param.Name);
         Assert.AreEqual(ParamOperation.Assign, param.Op);
         Assert.AreEqual("v2", param.Val);
      }

      [Test]
      public void Parse_StateSectionWithShorthandAction_TransitionActionProduced()
      {
         var input = "@machine m -> s  \n@state state1\n\trun\n\t-> blah";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(18, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 2), NLTkn(2),
            KeyTkn(TokenKeyword.Run, 3), NLTkn(3),
            OpTkn(TokenOperator.Transition, 4), IdTkn(41, 4, 4)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var action = ast.States[0].Run.Actions[0];

         Assert.AreEqual(ParserConstants.TransitionAction, action.Name);
         Assert.AreEqual(ParamOperation.Transition, action.Params[0].Op);
         Assert.AreEqual("blah", action.Params[0].Val);
      }

      [Test]
      public void Parse_StateAfterActions_TwoStatesProduced()
      {
         var input = "@machine m -> s  \n@state state1\n\trun\n\t-> blah \n\n@state state2";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(18, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 2), NLTkn(2),
            KeyTkn(TokenKeyword.Run, 3), NLTkn(3),
            OpTkn(TokenOperator.Transition, 4), IdTkn(41, 4, 4), NLTkn(4),
            NLTkn(5),
            KeyTkn(TokenKeyword.State, 6), IdTkn(55, 6, 6)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var state2 = ast.States[1];

         Assert.AreEqual("state2", state2.Name);
      }

      [Test]
      public void Parse_ActionWithMessageInOnSection_ActionHasMessage()
      {
         var input = "@machine m -> s  \n@state state1\n\ton\n\t'msg': act";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(18, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 2), NLTkn(2),
            KeyTkn(TokenKeyword.On, 3), NLTkn(3),
            ValTkn(38, 3, 4), OpTkn(TokenOperator.Assign, 4), IdTkn(44, 3, 4)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var act = ast.States[0].On.Actions[0];

         Assert.AreEqual("act", act.Name);
         Assert.AreEqual("msg", act.Message);
      }

      [Test]
      public void Parse_MessageFollowedBySectionKeyword_StateWillHaveTwoSections()
      {
         var input = "@machine m -> s  \n@state state1\n\t@on\n\t'msg': act\n@run\n\tblah";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(18, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 2), NLTkn(2),
            KeyTkn(TokenKeyword.On, 3), NLTkn(3),
            ValTkn(39, 3, 4), OpTkn(TokenOperator.Assign, 4), IdTkn(45, 3, 4), NLTkn(4),
            KeyTkn(TokenKeyword.Run, 5), NLTkn(5),
            IdTkn(55, 4, 6),
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var state = ast.States[0];

         Assert.AreEqual(1, state.On.Actions.Count);
         Assert.AreEqual(1, state.Run.Actions.Count);
      }

      [Test]
      public void Parse_ActionWithTransitionSyntacticSugar_ActionIsCorrect()
      {
         var input = "@machine m -> s  \n@state state1\n\ton\n\t'msg': -> thing";
         var tokens = new List<Token>
         {
            KeyTkn(TokenKeyword.Machine, 1), IdTkn(13, 1, 1), OpTkn(TokenOperator.Transition, 1), IdTkn(18, 1, 1), NLTkn(1),
            KeyTkn(TokenKeyword.State, 2), IdTkn(25, 6, 2), NLTkn(2),
            KeyTkn(TokenKeyword.On, 3), NLTkn(3),
            ValTkn(38, 3, 4), OpTkn(TokenOperator.Assign, 4), OpTkn(TokenOperator.Transition, 4), IdTkn(47, 5, 4)
         };
         var parser = new Parser();

         var ast = parser.Parse(tokens, input);
         var act = ast.States[0].On.Actions[0];

         Assert.AreEqual(ParserConstants.TransitionAction, act.Name);
         Assert.AreEqual(ParserConstants.DefaultParameterName, act.Params[0].Name);
      }
   }
}
