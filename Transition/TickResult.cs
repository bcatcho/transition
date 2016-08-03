namespace Transition
{
   public struct TickResult
   {
      public TickResultType ResultType;
      public int TransitionId;

      public static TickResult Yield()
      {
         return new TickResult
         {
            ResultType = TickResultType.Yield,
            TransitionId = 0
         };
      }

      public static TickResult Done()
      {
         return new TickResult
         {
            ResultType = TickResultType.Done,
            TransitionId = 0
         };
      }

      public static TickResult Transition(int transitionId)
      {
         return new TickResult
         {
            ResultType = TickResultType.Transition,
            TransitionId = transitionId
         };
      }

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
