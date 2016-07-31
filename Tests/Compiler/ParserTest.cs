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
         var testInput = @"@machine blah -> 'yar'";

         var tokens = Tokenize(testInput);

         Assert.AreEqual(TokenType.Identifier, tokens[1].TokenType);
         AssertTokenValue("blah", tokens[1], testInput); 
      }

      [Test]
      public void Tokenize_Machine_MachineHasCorrectNumberOfTokens()
      {
         var testInput = @"@machine blah -> 'b'";

         var tokens = Tokenize(testInput);

         Assert.AreEqual(4, tokens.Count);
      }

      [Test]
      public void Tokenize_MachineNoSpaces_MachineHasAllComponents()
      {
         var testInput = @"@machine blah->'b'";

         var tokens = Tokenize(testInput);

         Assert.AreEqual(4, tokens.Count);
         Assert.AreEqual(TokenType.Keyword, tokens[0].TokenType);
         Assert.AreEqual(TokenType.Identifier, tokens[1].TokenType);
         Assert.AreEqual(TokenType.Operator, tokens[2].TokenType);
         Assert.AreEqual(TokenType.TransitionValue, tokens[3].TokenType);
      }

      [Test]
      public void Tokenize_Machine_MachineHasCorrectTransitionValue()
      {
         var testInput = @"@machine blah -> 'yar'";

         var tokens = Tokenize(testInput);

         Assert.AreEqual(TokenType.TransitionValue, tokens[3].TokenType);
         AssertTokenValue("yar", tokens[3], testInput); 
      }

      [Test]
      public void Tokenize_MachineWithNewline_LastTokenIsNewline()
      {
         var testInput = @"@machine blah -> 'yar'
";

         var tokens = Tokenize(testInput);

         Assert.AreEqual(TokenType.NewLine, tokens[4].TokenType);
      }
   }
}