namespace Statescript.Compiler.AstNode
{
   /// <summary>
   /// State nodes encapsulate a number of sections. They must have a unique name within a machine.
   /// States do not need to have any sections at all.
   /// </summary>
   public class StateAstNode : AstNode
   {
      /// <summary>
      /// The unique (within a machine) name for this state.
      /// </summary>
      public string Name;

      /// <summary>
      /// The Enter section represents an ordered sequence of actions that will execute when this state is entered.
      /// </summary>
      public SectionAstNode Enter;

      /// <summary>
      /// The Exit section represents an ordered sequence of actions that will execute as this state is exited
      /// </summary>
      public SectionAstNode Exit;

      /// <summary>
      /// The run section represents an ordered sequence of actions that will be executed every tick. It is run after the Enter section.
      /// </summary>
      public SectionAstNode Run;


      /// <summary>
      /// Represents an unordered list of messages and actions to take.
      /// </summary>
      public SectionAstNode On;
   }
}
