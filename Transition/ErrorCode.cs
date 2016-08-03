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
      State_Enter_ActionDidNotReturnYield,
      /// <summary>
      /// An action must return yield inside of the Exit section of a state.
      /// </summary>
      State_Exit_ActionDidNotReturnYield,
   }
}
