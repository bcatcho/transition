namespace Transition
{
   /// <summary>
   /// An action is the most basic unit of execution. It must impelment OnTick
   /// and return a TickResult that instructs the State or Machine on how to proceede.
   /// 
   /// See <term>State</term> for the possible outcomes of the various TickResultTypes
   /// </summary>
   public abstract class Action<T> where T : Context
   {
      /// <remarks>
      /// Default constructor is necessary for runtime instantiation
      /// </remarks>
      public Action()
      {
      }

      /// <summary>
      /// Run this action and return an result.
      /// </summary>
      public TickResult Tick(T context)
      {
         return OnTick(context);
      }

      /// <summary>
      /// This method is called when an action is first entered before it's first tick
      /// </summary>
      /// <param name="context">Context.</param>
      public void EnterAction(T context)
      {
         OnEnterAction(context);
      }

      protected virtual void OnEnterAction(T context)
      {
      }

      /// <summary>
      /// Implement this to execute code when an action is Ticked. The TickResult will
      /// instruct the calling State or Machine on what to do next. See <term>State</term> for details.
      /// </summary>
      protected abstract TickResult OnTick(T context);

      /// <summary>
      /// Returns a transition result given a transition parameter. Return the result of this method
      /// to make an action transition.
      /// </summary>
      protected TickResult TransitionTo(TransitionDestination transition)
      {
         return TickResult.Transition(transition.StateId);
      }
   }
}
