using Transition.Compiler;

namespace Transition.Actions
{
   /// <summary>
   /// A built-in action that just returns a transition.
   /// </summary>
   [AltId(ParserConstants.TransitionAction)]
   public sealed class TransitionAction<T> : Action<T> where T : Context
   {
      /// <summary>
      /// The action will transition to this destination when ticked
      /// </summary>
      [DefaultParameterAttribute]
      public TransitionDestination Destination { get; set; }

      protected override TickResult OnTick(T context)
      {
         return TransitionTo(Destination);
      }
   }
}
