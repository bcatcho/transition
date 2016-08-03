namespace Transition
{
   /// <summary>
   /// The state of execution in an instance of a state machine.
   /// </summary>
   public class ExecutionState
   {
      /// <summary>
      /// The current index of the currently running action in a State's RunActions section.
      /// </summary>
      public int ActionIndex { get; set; }

      /// <summary>
      /// The Identifier of the currently active state.
      /// </summary>
      public int StateId { get; set; }
   }
}
