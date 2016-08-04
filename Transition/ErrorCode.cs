namespace Transition
{
   /// <summary>
   /// A list of all error codes that may be emitted durring compilation or execution
   /// </summary>
   public enum ErrorCode
   {
      None,
      /// <summary>
      /// An action must return yield inside of the Enter section of a state
      /// </summary>
      Exec_State_Enter_ActionDidNotReturnYield,
      /// <summary>
      /// An action must return yield inside of the Exit section of a state.
      /// </summary>
      Exec_State_Exit_ActionDidNotReturnYield,
      /// <summary>
      /// The action that runs when a machine is ticked for the first time (before the first state) 
      /// MUST return a transition result so that the machine can move to the first state.
      /// </summary>
      Exec_Machine_Tick_MachineActionMustReturnTransition,
      /// <summary>
      /// Occurs when a Machine moves to a state that does not exist during execution.
      /// </summary>
      Exec_Machine_Transition_DestinationStateDoesNotExist
   }
}
