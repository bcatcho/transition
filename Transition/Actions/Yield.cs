namespace Transition.Actions
{
   /// <summary>
   /// A built-in action that causes the @run section to loop.
   /// This will cause the machine to yield but move to the next
   /// action on the next tick
   /// </summary>
   public sealed class Yield<T> : Action<T> where T : Context
   {
      protected override TickResult OnTick(T context)
      {
         return TickResult.YieldDone();
      }
   }
}
