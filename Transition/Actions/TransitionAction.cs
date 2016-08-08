using Transition.Compiler;

namespace Transition.Actions
{
   /// <summary>
   /// A built-in action that just returns a transition.
   /// </summary>
   [AltId(ParserConstants.TransitionAction)]
   public sealed class TransitionAction<T> : Action<T> where T : Context
   {
      protected override TickResult OnTick(T context)
      {
         return TransitionTo(0);
      }
   }
}
