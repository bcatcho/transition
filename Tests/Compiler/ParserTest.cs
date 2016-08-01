using NUnit.Framework;
using Statescript.Compiler;
using System.Collections.Generic;
using System;

namespace Tests.Compiler
{
   [TestFixture]
   public class ParserTest
   {
      private List<Token> Tokenize(string input) {
         var parser = new Tokenizer();
         parser.Init();
         var tokens = parser.Tokenize(input.ToCharArray(), input.Length);
         parser.Finish();
         return tokens;
      }

      private void AssertTokenValue(string expected, Token token, string data)
      {
         Assert.AreEqual(expected, data.Substring(token.StartIndex, token.Length));
      }

      [Test]
      public void Tokenize_Machine_MachineHasCorrectName()
      {
         var input = @"@machine blah -> 'yar'";

         var tokens = Tokenize(input);

         Assert.AreEqual(TokenType.Identifier, tokens[1].TokenType);
         AssertTokenValue("blah", tokens[1], input); 
      }

      [Test]
      public void Tokenize_Machine_MachineHasCorrectNumberOfTokens()
      {
         var input = @"@machine blah -> 'b'";

         var tokens = Tokenize(input);

         Assert.AreEqual(4, tokens.Count);
      }

      [Test]
      public void Tokenize_MachineNoSpaces_MachineHasAllComponents()
      {
         var input = @"@machine blah->'b'";

         var tokens = Tokenize(input);

         Assert.AreEqual(4, tokens.Count);
         Assert.AreEqual(TokenType.Keyword, tokens[0].TokenType);
         Assert.AreEqual(TokenType.Identifier, tokens[1].TokenType);
         Assert.AreEqual(TokenType.Operator, tokens[2].TokenType);
         Assert.AreEqual(TokenType.TransitionValue, tokens[3].TokenType);
      }

      [Test]
      public void Tokenize_Machine_MachineHasCorrectTransitionValue()
      {
         var input = @"@machine blah -> 'yar'";

         var tokens = Tokenize(input);

         Assert.AreEqual(TokenType.TransitionValue, tokens[3].TokenType);
         AssertTokenValue("yar", tokens[3], input); 
      }

      [Test]
      public void Tokenize_MachineWithNewline_LastTokenIsNewline()
      {
         var input = "@machine blah -> 'yar'\n";

         var tokens = Tokenize(input);

         Assert.AreEqual(TokenType.NewLine, tokens[4].TokenType);
      }

      [Test]
      public void Tokenize_MultipleNewLines_ProduceMultipleNewLineTokens()
      {
         var input = "@machine blah -> 'yar'\n\n\n";

         var tokens = Tokenize(input);

         Assert.AreEqual(7, tokens.Count);
         Assert.AreEqual(TokenType.NewLine, tokens[4].TokenType);
      }

      [Test]
      public void Tokenize_State_KeywordTokenIsProducedWithId()
      {
         var input = "@state StateIdentifier";

         var tokens = Tokenize(input);

         Assert.AreEqual(2, tokens.Count);
         Assert.AreEqual(TokenType.Keyword, tokens[0].TokenType);
         Assert.AreEqual(TokenType.Identifier, tokens[1].TokenType);
         AssertTokenValue("StateIdentifier", tokens[1], input);
      }
   }
}