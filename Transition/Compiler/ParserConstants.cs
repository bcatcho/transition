namespace Transition.Compiler
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
      public const string TransitionAction = "$trans";

      /// <summary>
      /// This keyword is used as the identifier for a default parameter in the abstract syntax tree.
      /// For example, an action called "log" that takes a single parameter "message" can be written like
      ///  "log message:'blah'" or "log 'blah'" if the "DefaultParameterAttribute is applied to "message".
      /// This is a bit of syntactic sugar that makes some actions more readable. However the abstract syntax tree
      /// requires an Identifier for each parameter. In the second example this value is used.
      /// </summary>
      public const string DefaultParameterIdentifier = "$default";
   }
}
