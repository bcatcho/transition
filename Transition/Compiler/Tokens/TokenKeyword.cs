namespace Transition.Compiler.Tokens
{

   /// <summary>
   /// The specific keyword found when tokenizing. This is identified during
   /// lexical analysis as an optimization.
   /// </summary>
   public enum TokenKeyword
   {
      None,
      Enter,
      On,
      Exit,
      Run,
      Machine,
      State
   }
}
