using System.Collections.Generic;

namespace Transition
{
   /// <summary>
   /// An executable state machine. 
   /// </summary>
   public class Machine<T> where T : Context
   {
      /// <summary>
      /// The unique Name of the machine
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// All of the states that make up the machine. States are indexed
      /// by their position in this list.
      /// </summary>
      public List<State<T>> States { get; private set; }

      /// <summary>
      /// A lookup table of actions to run when a message is recieved
      /// </summary>
      public Dictionary<string, Action<T>> OnActions { get; private set; }

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
         OnActions = new Dictionary<string, Action<T>>(0);
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

         TickResult result;
         // see if the current state can handle it first
         var currentState = CurrentState(context);
         if (currentState != null && currentState.CanHandleMessage(message)) {
            result = currentState.SendMessage(context, message);
         } else {
            // otherwise handle it ourselves
            Action<T> handler;
            if (OnActions.TryGetValue(message.Key, out handler)) {
               context.Message = message;
               result = handler.Tick(context);
               context.Message = null;
            } else {
               // no one can handle the message so exit early
               return;
            }
         } 
         if (result.ResultType == TickResultType.Transition) {
            Transition(context, result.TransitionId);
         } else {
            context.RaiseError(ErrorCode.Exec_State_SendMessage_MessageHandlerDidNotReturnTransitionOrDone);
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

      /// <summary>
      /// Adds an action to the OnActions collection
      /// </summary>
      public void AddOnAction(string key, Action<T> action)
      {
         OnActions.Add(key, action);
      }
   }
}
