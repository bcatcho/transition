namespace Transition
{
   /// <summary>
   /// Use this class to generate a Transition TickResult within an action. The StateId is immutible as it is set
   /// by the compiler
   /// </summary>
   public class TransitionDestination
   {
      /// <summary>
      /// The destination stateId. This property should be considered immutable
      /// </summary>
      public readonly int StateId;

      public TransitionDestination(int stateId)
      {
         StateId = stateId;
      }
   }
}
