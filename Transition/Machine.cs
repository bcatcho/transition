using System.Collections.Generic;

namespace Transition
{
   /// <summary>
   /// An executable state machine. 
   /// </summary>
   public class Machine<T> where T : Context
   {
      /// <summary>
      /// The unique Id (or name) of the machine
      /// </summary>
      public string Identifier { get; set; }

      /// <summary>
      /// All of the states that make up the machine. States are indexed
      /// by their position in this list.
      /// </summary>
      public List<State<T>> States { get; private set; }

      /// <summary>
      /// The first action that the state takes on it's first tick before any state is executed.
      /// It MUST transition to the first state.
      /// </summary>
      /// <remarks>
      /// The Parser ensures that this exists and so null checking is unecessary.
      /// </remarks>
      public Action<T> EnterAction { get; set; }

      public Machine()
      {
         States = new List<State<T>>();
      }

      /// <summary>
      /// Run the state machine for one tick. The resulting state of the Machine will be
      /// stored in the context.
      /// </summary>
      public void Tick(T context)
      {
         context.ResetError();
         if (context.StateId == -1) {
            var result = EnterAction.Tick(context);
            if (result.ResultType != TickResultType.Transition) {
               context.RaiseError(ErrorCode.Exec_Machine_Tick_MachineActionMustReturnTransition);
               return;
            }
            Transition(context, result.TransitionId);
         } else {
            var currentState = CurrentState(context);
            if (currentState == null) {
               context.RaiseError(ErrorCode.Exec_Machine_Tick_CurrentStateDoesNotExist);
               return;
            }
            var result = currentState.Tick(context);
            if (result.ResultType == TickResultType.Transition) {
               Transition(context, result.TransitionId);
            }
         }
      }

      /// <summary>
      /// Sends the message to the active state in the machine. May result in a state transition.
      /// </summary>
      public void SendMessage(T context, MessageEnvelope message)
      {
         var currentState = CurrentState(context);
         if (currentState == null)
            return;

         var result = currentState.SendMessage(context, message);
         if (result.ResultType == TickResultType.Transition) {
            Transition(context, result.TransitionId);
         }
      }

      /// <summary>
      /// Add a state to the machine.
      /// </summary>
      public void AddState(State<T> state)
      {
         States.Add(state);
      }

      private void Transition(T context, int destinationStateId)
      {
         var currentState = CurrentState(context);
         if (currentState != null) {
            currentState.Exit(context);
         }
         context.StateId = destinationStateId;
         currentState = CurrentState(context);
         // ensure we land on a state
         if (currentState == null) {
            context.RaiseError(ErrorCode.Exec_Machine_Transition_DestinationStateDoesNotExist);
            return;
         }
         currentState.Enter(context);
      }

      private State<T> CurrentState(T context)
      {
         var stateId = context.StateId;
         if (stateId < 0 || stateId >= States.Count)
            return null;

         return States[stateId];
      }
   }
}
