namespace Statescript.Compiler
{
   /// <summary>
   /// A collection of keywords used in parsing and compiling
   /// </summary>
   public static class ParserConstants
   {
      /// <summary>
      /// The action name for the syntatic sugar transition action.
      /// Eg. "-> 'state'" becomes "$trans -> 'state'" so the compiler can
      /// use the built in TransitionAction.  
      /// </summary>
      public static readonly string TransitionAction = "$trans";
   }
}
