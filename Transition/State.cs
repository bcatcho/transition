using System.Collections.Generic;

namespace Transition
{
   /// <summary>
   /// A State is an executable representation of the StateAstNode. It has four sections which hold
   /// a sequence or unordered collection of actions. When a state is ticked it will run actions from
   /// the "Run" section. An action in the run section does not have to finish in one tick. 
   /// 
   /// When a state is entered or exited it runs all actions in "EnterActions" or "ExitActions"
   /// respectively. An action shall not transition, yield or loop in either of these phases.
   /// </summary>
   public class State<T> where T: Context
   {
      /// <summary>
      /// The unique name of a state within a machine
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// An ordered list of actions to be run every tick
      /// </summary>
      public List<Action<T>> RunActions { get; private set; }

      /// <summary>
      /// An ordered list of actions to run when the state is entered
      /// </summary>
      public List<Action<T>> EnterActions { get; private set; }

      /// <summary>
      /// An ordered list of actions to be run when the state is exited
      /// </summary>
      public List<Action<T>> ExitActions { get; private set; }

      /// <summary>
      /// A lookup table of actions to run when a message is recieved
      /// </summary>
      public Dictionary<string, Action<T>> OnActions { get; private set; }

      public State()
      {
         OnActions = new Dictionary<string, Action<T>>(0);
      }

      /// <summary>
      /// Running the Tick command will run actions in the "RunActions" list starting from the first.
      /// If no Actions are present the state will return yield. This allows for passive states that 
      /// wait for messages or other external forces to act.
      /// Any action that is run will return one of four TickResultType enums with different side effects.
      /// 
      /// <term>Yield</term> causes State to immediately return yield itself without incrementing the
      /// active action index in the execution context. This allows a long running action (across multiple ticks).
      /// 
      /// <term>Done</term> causes the State to increment the active action index and immediately execute 
      /// the next action (if availabile). If none is available the State returns yield.
      /// 
      /// <term>Loop</term> causes the State to reset the active action index to zero and immedeitely yield itself.
      /// The next time the State is ticked it starts over at the first action (index 0).
      /// 
      /// <term>Transition</term> causes the State to return the same transition result. The parent Machine will
      /// immediately transition to the new state identified by result.TransitionId;
      /// </summary>
      /// <param name="context">Context.</param>
      public TickResult Tick(T context)
      {
         if (RunActions == null) {
            return TickResult.Yield();
         }

         TickResult result;
         Action<T> action;
         // if ticking first action for the first time, enter it
         if (context.ActionIndex == -1) {
            context.ActionIndex++;
            action = CurrentAction(context);
            action.EnterAction(context);
         } else {
            action = CurrentAction(context);
         }
         while (action != null) {
            result = action.Tick(context);

            switch (result.ResultType) {
               case TickResultType.Yield:
                  // current action still has more work to do next tick
                  return TickResult.Yield();
               case TickResultType.Done:
                  // if the last action finished, advance
                  action = AdvanceAction(context);
                  break;
               case TickResultType.YieldDone:
                  // yield but advance the action
                  AdvanceAction(context);
                  return TickResult.Yield();
               case TickResultType.Loop:
                  // reset and yield to avoid infinite loops
                  ResetForLooping(context);
                  return TickResult.Yield();
               case TickResultType.Transition:
                  // exit and let machine handle transition
                  return result;
               default:
                  return TickResult.Yield();
            }
         }

         return TickResult.Yield();
      }

      /// <summary>
      /// Executes all actions in the "EnterActions" in order section when the state is entered.
      /// Each action must finish in one tick and return done.
      /// </summary>
      public void Enter(T context)
      {
         context.ActionIndex = -1;

         if (EnterActions == null)
            return;

         TickResult result;
         for (int i = 0; i < EnterActions.Count; ++i) {
            result = EnterActions[i].Tick(context);
            if (result.ResultType != TickResultType.Done) {
               context.RaiseError(ErrorCode.Exec_State_Enter_ActionDidNotReturnYield);
            }
         }
      }

      /// <summary>
      /// Executes all actions in the "ExitActions" in order section when the state is exited.
      /// Each action must finish in one tick and return done.
      /// </summary>
      public void Exit(T context)
      {
         if (ExitActions == null)
            return;

         TickResult result;
         for (int i = 0; i < ExitActions.Count; ++i) {
            result = ExitActions[i].Tick(context);
            if (result.ResultType != TickResultType.Done) {
               context.RaiseError(ErrorCode.Exec_State_Exit_ActionDidNotReturnYield);
            }
         }
      }

      /// <summary>
      /// Send a message to a state. This has the possiblity of causing the state to transition.
      /// A state does not have to handle every message sent to it.
      /// </summary>
      public TickResult SendMessage(T context, MessageEnvelope message)
      {
         var messageHandler = GetMessageHandler(message);
         if (messageHandler != null) {
            context.Message = message;
            var result = messageHandler.Tick(context);
            context.Message = null;

            switch (result.ResultType) {
               case TickResultType.Done:
                  return result;
               case TickResultType.Transition:
                  return result;
               default:
                  // all other results are an error
                  context.RaiseError(ErrorCode.Exec_State_SendMessage_MessageHandlerDidNotReturnTransitionOrDone);
                  return TickResult.Done();
            }
         }

         return TickResult.Done();
      }

      public bool CanHandleMessage(MessageEnvelope message)
      {
         return OnActions.ContainsKey(message.Key);
      }

      private Action<T> GetMessageHandler(MessageEnvelope message)
      {
         Action<T> result;
         return OnActions.TryGetValue(message.Key, out result) ? result : null;
      }

      private Action<T> CurrentAction(Context context)
      {
         if (context.ActionIndex < RunActions.Count) {
            return RunActions[context.ActionIndex];
         }

         return null;
      }

      private Action<T> AdvanceAction(T context)
      {
         context.ActionIndex++;
         var nextAction = CurrentAction(context);
         if (nextAction != null) {
            nextAction.EnterAction(context);
         }
         return nextAction;
      }

      private void ResetForLooping(T context)
      {
         context.ActionIndex = 0;
      }

      /// <summary>
      /// Appends an action to the RunActions list
      /// </summary>
      public void AddRunAction(Action<T> action)
      {
         if (RunActions == null) {
            RunActions = new List<Action<T>>(1);
         }
         RunActions.Add(action);
      }

      /// <summary>
      /// Appends an action to the EnterActions list
      /// </summary>
      public void AddEnterAction(Action<T> action)
      {
         if (EnterActions == null) {
            EnterActions = new List<Action<T>>(1);
         }
         EnterActions.Add(action);
      }

      /// <summary>
      /// Appends an action to the ExitActions list
      /// </summary>
      public void AddExitAction(Action<T> action)
      {
         if (ExitActions == null) {
            ExitActions = new List<Action<T>>(1);
         }
         ExitActions.Add(action);
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
