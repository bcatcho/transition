// This file is AUTOGENERATED with RAGEL
// !!DO NOT EDIT!! Change the RL file and compile with Ragel
// http://www.colm.net/open-source/ragel/
using System;
using System.Collections.Generic;

namespace Statescript.Compiler
{
   public enum TokenType
   {
     Keyword,
     Identifier,
     Value,
     TransitionValue,
     MessageValue,
     Operator,
     NewLine
   }

   public enum TokenOperator
   {
     Set,
     Transition
   }

   public struct Token
   {
     public int StartIndex;
     public int Length;
     public int LineNumber;
     public TokenType TokenType;
     public TokenOperator Operator;
   }

   public class Tokenizer
   {
      int _lineNumber = 0;
      bool _tokenUncommitted;
      int _tokenStart { get { return _token.StartIndex; } }
      Token _token;
      private List<Token> _tokens;
      char[] _data;
      // ragel properties
      private int cs;
      int p;

      private void StartToken(TokenType tokenType)
      {
        _token = new Token {
            LineNumber = _lineNumber,
            StartIndex = p,
            TokenType = tokenType
        };
        _tokenUncommitted = true;
      }

      private void StartOperatorToken(TokenOperator tokenOperator)
      {
        _token = new Token {
            LineNumber = _lineNumber,
            StartIndex = p,
            Operator = tokenOperator,
            TokenType = TokenType.Operator,
        };
        _tokenUncommitted = true;
      }

      private void log(string msg) {
        Console.WriteLine(string.Format("{0} {1}", p, msg));
      }

      private void logEnd(string msg) {
        var token = new String(_data, _tokenStart, p - _tokenStart);
        Console.WriteLine(string.Format("{0} {1}: {2}", p, msg, token));
      }

      private void EmitToken() {
        _token.Length = p - _tokenStart;
        _tokens.Add(_token);
        _tokenUncommitted = false;
      }

      private void EmitNewLine() {
        _token.TokenType = TokenType.NewLine;
        _tokens.Add(_token);
        _tokenUncommitted = false;
      }

      private void CommitLastToken() {
        if (_tokenUncommitted) {
          // update length in case the file ended early
          _token.Length = p -_token.StartIndex;
          _tokens.Add(_token);
          _tokenUncommitted = false;
        }
      }

      %%{
        machine Tokenizer;
        include TokenizerDef "TokenizerDef.rl";
        write data;
      }%%

      public void Init()
      {
         %% write init;
      }

      public List<Token> Tokenize(char[] data, int len)
      {
         if (_tokens == null) {
           _tokens = new List<Token>(128);
         }
         _tokens.Clear();
         _lineNumber = 1; // start at line 1 like most text editors
         _data = data;
         p = 0;
         int pe = len;
         //int eof = len;
         %% write exec;
         CommitLastToken();
         return _tokens;
      }

      public bool Finish()
      {
         return (cs >= Tokenizer_first_final);
      }
   }
}