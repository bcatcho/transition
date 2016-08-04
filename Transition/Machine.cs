using System.Collections.Generic;

namespace Transition
{
   public class Machine
   {
      /// <summary>
      /// All of the states that make up the machine. States are indexed
      /// by their position in this list.
      /// </summary>
      public List<State> States { get; private set; }


      /// <summary>
      /// The first action that the state takes on it's first tick before any state is executed.
      /// It MUST transition to the first state.
      /// </summary>
      /// <remarks>
      /// The Parser ensures that this exists and so null checking is unecessary.
      /// </remarks>
      public Action EnterAction { get; set; }

      public Machine()
      {
         States = new List<State>();
      }

      /// <summary>
      /// Run the state machine for one tick. The resulting state of the Machine will be
      /// stored in the context.
      /// </summary>
      public void Tick(Context context)
      {
         context.ResetError();
         if (context.ExecState.StateId == -1) {
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

      private void Transition(Context context, int destinationStateId)
      {
         var currentState = CurrentState(context);
         if (currentState != null) {
            currentState.Exit(context);
         }
         context.ExecState.StateId = destinationStateId;
         currentState = CurrentState(context);
         // ensure we land on a state
         if (currentState == null) {
            context.RaiseError(ErrorCode.Exec_Machine_Transition_DestinationStateDoesNotExist);
            return;
         }
         currentState.Enter(context);
      }

      private State CurrentState(Context context)
      {
         var stateId = context.ExecState.StateId;
         if (stateId < 0 || stateId >= States.Count)
            return null;

         return States[stateId];
      }

      /// <summary>
      /// Add a state to the machine.
      /// </summary>
      public void AddState(State state)
      {
         States.Add(state);
      }
   }
}
