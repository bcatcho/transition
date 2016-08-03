namespace Transition
{
   /// <summary>
   /// Actions and States return a TickResult to indicate how a State or Machine
   /// should proceede. See <term>State</term> for details.
   /// </summary>
   public struct TickResult
   {
      /// <summary>
      /// The type of the result.
      /// </summary>
      public TickResultType ResultType;

      /// <summary>
      /// The transition to take if the ResultType was Transition.
      /// </summary>
      public int TransitionId;

      /// <summary>
      /// Helper method to build a yield result
      /// </summary>
      public static TickResult Yield()
      {
         return new TickResult
         {
            ResultType = TickResultType.Yield,
            TransitionId = 0
         };
      }

      /// <summary>
      /// Helper method to build a done result
      /// </summary>
      public static TickResult Done()
      {
         return new TickResult
         {
            ResultType = TickResultType.Done,
            TransitionId = 0
         };
      }

      /// <summary>
      /// Helper method to build a transition result
      /// </summary>
      public static TickResult Transition(int transitionId)
      {
         return new TickResult
         {
            ResultType = TickResultType.Transition,
            TransitionId = transitionId
         };
      }

      /// <summary>
      /// Helper method to build a loop result
      /// </summary>
      public static TickResult Loop()
      {
         return new TickResult
         {
            ResultType = TickResultType.Loop,
            TransitionId = 0
         };
      }
   }
}
