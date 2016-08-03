namespace Transition
{
   public abstract class Action
   {
      public TickResult Tick(Context context)
      {
         return OnTick(context);
      }

      protected abstract TickResult OnTick(Context context);
   }
}
