namespace Transition.Actions
{
   /// <summary>
   /// A built-in action that causes the @run section to loop.
   /// </summary>
   public sealed class Loop<T> : Action<T> where T : Context
   {
      protected override TickResult OnTick(T context)
      {
         return TickResult.Loop();
      }
   }
}
