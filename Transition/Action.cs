namespace Transition
{
   /// <summary>
   /// An action is the most basic unit of execution. It must impelment OnTick
   /// and return a TickResult that instructs the State or Machine on how to proceede.
   /// 
   /// See <term>State</term> for the possible outcomes of the various TickResultTypes
   /// </summary>
   public abstract class Action
   {
      /// <summary>
      /// Run this action and return an result.
      /// </summary>
      public TickResult Tick(Context context)
      {
         return OnTick(context);
      }

      /// <summary>
      /// Implement this to execute code when an action is Ticked. The TickResult will
      /// instruct the calling State or Machine on what to do next. See <term>State</term> for details.
      /// </summary>
      protected abstract TickResult OnTick(Context context);
   }
}
