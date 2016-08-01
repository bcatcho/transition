namespace Statescript.Compiler
{
   /// <summary>
   /// A product of lexical analysis on a string of input. Emitted by the Tokenizer
   /// </summary>
   /// <remarks>
   /// A struct was choosen to save heap when analyzing many files.
   /// This struct just acts as a index of the original input. The value of the token can be found
   /// by taking a substring of the original input from startIndex to startIndex+Length inclusive.
   /// </remarks>
   public struct Token
   {
      /// <summary>
      /// The start index of the first character of the token.
      /// </summary>
      public int StartIndex;

      /// <summary>
      /// The length of the token value.
      /// </summary>
      public int Length;

      /// <summary>
      /// The line number on which the token started.
      /// </summary>
      public int LineNumber;

      /// <summary>
      /// The type of the token.
      /// </summary>
      public TokenType TokenType;

      /// <summary>
      /// The type of operator (if this token is an operator)
      /// </summary>
      public TokenOperator Operator;
   }
}
