using NUnit.Framework;
using Transition.Compiler;
using System.Collections.Generic;
using System;
using Transition.Compiler.Tokens;

namespace Tests.Compiler
{
   /// <summary>
   /// Because the Scanner is built using ragel we can only test at a functional level.
   /// </summary>
   [TestFixture]
   public class ScannerTests
   {
      private List<Token> Scan(string input)
      {
         var scanner = new Scanner();
         var tokens = scanner.Scan(input.ToCharArray(), input.Length);
         return tokens;
      }

      private void AssertTokenValue(string expected, Token token, string data)
      {
         Assert.AreEqual(expected, data.Substring(token.StartIndex, token.Length));
      }

      [Test]
      public void Scan_Blank_NoTokensProduced()
      {
         var input = "";
         var tokens = Scan(input);

         Assert.AreEqual(0, tokens.Count); // two keywords and newline
      }

      [Test]
      public void Scan_Machine_MachineHasCorrectName()
      {
         var input = @"@machine blah -> yar";

         var tokens = Scan(input);

         Assert.AreEqual(TokenType.Identifier, tokens[1].TokenType);
         AssertTokenValue("blah", tokens[1], input);
      }

      [Test]
      public void Scan_Machine_MachineHasCorrectNumberOfTokens()
      {
         var input = @"@machine blah -> b";

         var tokens = Scan(input);

         Assert.AreEqual(4, tokens.Count);
      }

      [Test]
      public void Scan_MachineNoSpaces_MachineHasAllComponents()
      {
         var input = @"@machine blah->b";

         var tokens = Scan(input);

         Assert.AreEqual(4, tokens.Count);
         Assert.AreEqual(TokenType.Keyword, tokens[0].TokenType);
         Assert.AreEqual(TokenType.Identifier, tokens[1].TokenType);
         Assert.AreEqual(TokenType.Operator, tokens[2].TokenType);
         Assert.AreEqual(TokenType.Identifier, tokens[3].TokenType);
      }

      [Test]
      public void Scan_Machine_MachineHasCorrectTransitionValue()
      {
         var input = @"@machine blah -> yar";

         var tokens = Scan(input);

         Assert.AreEqual(TokenType.Identifier, tokens[3].TokenType);
         AssertTokenValue("yar", tokens[3], input);
      }

      [Test]
      public void Scan_MachineWithNewline_LastTokenIsNewline()
      {
         var input = "@machine blah -> yar\n";

         var tokens = Scan(input);

         Assert.AreEqual(TokenType.NewLine, tokens[4].TokenType);
      }

      [Test]
      public void Scan_MultipleNewLines_ProduceSingleNewLineToken()
      {
         var input = "@machine blah -> yar\n\n\n";

         var tokens = Scan(input);

         Assert.AreEqual(5, tokens.Count);
         Assert.AreEqual(TokenType.NewLine, tokens[4].TokenType);
      }

      [Test]
      public void Scan_State_KeywordTokenIsProducedWithId()
      {
         var input = "@state StateName";

         var tokens = Scan(input);

         Assert.AreEqual(2, tokens.Count);
         Assert.AreEqual(TokenType.Keyword, tokens[0].TokenType);
         Assert.AreEqual(TokenType.Identifier, tokens[1].TokenType);
         AssertTokenValue("StateName", tokens[1], input);
      }

      [Test]
      public void Scan_MultipleKeywordWithIds_TwoKeywordWithIdsProduced()
      {
         var input = "@state stateId1\n@state stateId2";

         var tokens = Scan(input);

         Assert.AreEqual(5, tokens.Count);
         Assert.AreEqual(TokenType.Keyword, tokens[0].TokenType);
         Assert.AreEqual(TokenType.Identifier, tokens[1].TokenType);
         AssertTokenValue("stateId1", tokens[1], input);
         AssertTokenValue("stateId2", tokens[4], input);
      }

      [Test]
      public void Scan_KeywordLine_KeywordTokenProduced()
      {
         var input = "@on";

         var tokens = Scan(input);

         Assert.AreEqual(1, tokens.Count);
         Assert.AreEqual(TokenType.Keyword, tokens[0].TokenType);
      }

      [Test]
      public void Scan_TwoKeywordLines_TwoKeywordTokenProduced()
      {
         var input = "@on\n@exit";

         var tokens = Scan(input);

         Assert.AreEqual(3, tokens.Count);
         Assert.AreEqual(TokenType.Keyword, tokens[0].TokenType);
         Assert.AreEqual(TokenType.Keyword, tokens[2].TokenType);
      }

      [Test]
      public void Scan_AllKeywords_KeywordsCorrectlyIdentified()
      {
         var input = "@machine\n@state\n@enter\n@exit\n@run\n@on";

         var tokens = Scan(input);

         Assert.AreEqual(11, tokens.Count);
         Assert.AreEqual(TokenKeyword.Machine, tokens[0].Keyword);
         Assert.AreEqual(TokenKeyword.State, tokens[2].Keyword);
         Assert.AreEqual(TokenKeyword.Enter, tokens[4].Keyword);
         Assert.AreEqual(TokenKeyword.Exit, tokens[6].Keyword);
         Assert.AreEqual(TokenKeyword.Run, tokens[8].Keyword);
         Assert.AreEqual(TokenKeyword.On, tokens[10].Keyword);
      }

      [Test]
      public void Scan_Message_TokensAreCorrect()
      {
         var input = "@machine MachineName\n@state stateName\n@on\n'msg':action";

         var tokens = Scan(input);

         Assert.AreEqual(11, tokens.Count);
         Assert.AreEqual(TokenType.Value, tokens[8].TokenType);
         Assert.AreEqual(TokenType.Operator, tokens[9].TokenType);
         Assert.AreEqual(TokenType.Identifier, tokens[10].TokenType);
      }

      [Test]
      public void Scan_TaskWithNoParams_OneIdentifierProduced()
      {
         var input = "dothing";

         var tokens = Scan(input);

         Assert.AreEqual(1, tokens.Count);
         Assert.AreEqual(TokenType.Identifier, tokens[0].TokenType);
         AssertTokenValue("dothing", tokens[0], input);
      }

      [Test]
      public void Scan_TaskWithOneAssignParam_OneAssignParamProduced()
      {
         var input = "task set:'12341 asdf b'";

         var tokens = Scan(input);

         Assert.AreEqual(4, tokens.Count);
         Assert.AreEqual(TokenType.Identifier, tokens[0].TokenType);
         Assert.AreEqual(TokenType.Identifier, tokens[1].TokenType);
         Assert.AreEqual(TokenType.Operator, tokens[2].TokenType);
         Assert.AreEqual(TokenType.Value, tokens[3].TokenType);
      }

      [Test]
      public void Scan_TaskWithDefaultPAram_OneAssignParamProduced()
      {
         var input = "@machine x\n@state y\n@on\ntask 'blah'";

         var tokens = Scan(input);

         Assert.AreEqual(10, tokens.Count);
         Assert.AreEqual(TokenType.Value, tokens[9].TokenType);
      }

      [Test]
      public void Scan_TaskCommentTask_TwoTasksProduced()
      {
         var input = "task\n#task\ntask";

         var tokens = Scan(input);

         Assert.AreEqual(3, tokens.Count);
         Assert.AreEqual(TokenType.Identifier, tokens[0].TokenType);
         Assert.AreEqual(TokenType.NewLine, tokens[1].TokenType);
         Assert.AreEqual(TokenType.Identifier, tokens[2].TokenType);
      }

      [Test]
      public void Scan_TaskAndCommentInSameLine_OneTaskProduced()
      {
         var input = "task#Comment";

         var tokens = Scan(input);

         Assert.AreEqual(1, tokens.Count);
         Assert.AreEqual(TokenType.Identifier, tokens[0].TokenType);
      }

      [Test]
      public void Scan_TaskAndCommentNewLineTask_TwoTasksProduced()
      {
         var input = "task#comment\ntask2";

         var tokens = Scan(input);

         Assert.AreEqual(3, tokens.Count);
         Assert.AreEqual(TokenType.Identifier, tokens[0].TokenType);
         Assert.AreEqual(TokenType.NewLine, tokens[1].TokenType);
         Assert.AreEqual(TokenType.Identifier, tokens[2].TokenType);
      }

      [Test]
      public void Scan_CommentOnly_NoTaskProduced()
      {
         var input = "#hello";

         var tokens = Scan(input);

         Assert.AreEqual(0, tokens.Count);
      }

      [Test]
      public void Scan_EmtpyLineThenTask_TaskIsProduced()
      {
         var input = "\ntask";

         var tokens = Scan(input);

         Assert.AreEqual(1, tokens.Count);
         Assert.AreEqual(TokenType.Identifier, tokens[0].TokenType);
      }

      [Test]
      public void Scan_BlankLineThenTask_TaskIsProduced()
      {
         var input = "\t \ntask";

         var tokens = Scan(input);

         Assert.AreEqual(1, tokens.Count);
         Assert.AreEqual(TokenType.Identifier, tokens[0].TokenType);
      }

      [Test]
      public void Scan_CommentLineThenTask_TaskIsProduced()
      {
         var input = "#hello\ntask";

         var tokens = Scan(input);

         Assert.AreEqual(2, tokens.Count);
         Assert.AreEqual(TokenType.Identifier, tokens[1].TokenType);
      }

      [Test]
      public void DidReachEndOfOutput_ValidInput_ReturnsTrue()
      {
         var input = "@machine mach";
         var scanner = new Scanner();

         scanner.Scan(input.ToCharArray(), input.Length);

         Assert.IsTrue(scanner.DidReachEndOfInput());
      }

      [Test]
      public void DidReachEndOfOutput_InvalidInput_ReturnsFalse()
      {
         var input = "@ma ch ine blah";
         var scanner = new Scanner();

         scanner.Scan(input.ToCharArray(), input.Length);

         Assert.IsFalse(scanner.DidReachEndOfInput());
      }
   }
}
