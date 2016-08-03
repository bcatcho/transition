using System.Collections.Generic;

namespace Transition
{
   public class State
   {
      public List<Action> RunActions { get; private set; }

      public List<Action> EnterActions { get; private set; }

      public List<Action> ExitActions { get; private set; }

      public List<Action> OnActions { get; private set; }

      public TickResult Tick(Context context)
      {
         if (RunActions == null) {
            return TickResult.Yield();
         }

         TickResult result;
         var action = CurrentAction(context);
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

      public void Enter(Context context)
      {
         if (EnterActions == null)
            return;

         TickResult result;
         for (int i = 0; i < EnterActions.Count; ++i) {
            result = EnterActions[i].Tick(context);
            if (result.ResultType != TickResultType.Done) {
               context.RaiseError(ErrorCode.State_Enter_ActionDidNotReturnYield);
            }
         }
      }

      public void Exit(Context context)
      {
         if (ExitActions == null)
            return;

         TickResult result;
         for (int i = 0; i < ExitActions.Count; ++i) {
            result = ExitActions[i].Tick(context);
            if (result.ResultType != TickResultType.Done) {
               context.RaiseError(ErrorCode.State_Exit_ActionDidNotReturnYield);
            }
         }
      }

      private Action CurrentAction(Context context)
      {
         if (context.ExecState.ActionIndex < RunActions.Count) {
            return RunActions[context.ExecState.ActionIndex];
         }

         return null;
      }

      private Action AdvanceAction(Context context)
      {
         context.ExecState.ActionIndex++;
         return CurrentAction(context);
      }

      private void ResetForLooping(Context context)
      {
         context.ExecState.ActionIndex = 0;
      }

      public void AddRunAction(Action action)
      {
         if (RunActions == null) {
            RunActions = new List<Action>(1);
         }
         RunActions.Add(action);
      }

      public void AddEnterAction(Action action)
      {
         if (EnterActions == null) {
            EnterActions = new List<Action>(1);
         }
         EnterActions.Add(action);
      }

      public void AddExitAction(Action action)
      {
         if (ExitActions == null) {
            ExitActions = new List<Action>(1);
         }
         ExitActions.Add(action);
      }

      public void AddOnAction(Action action)
      {
         if (OnActions == null) {
            OnActions = new List<Action>(1);
         }
         OnActions.Add(action);
      }
   }
}
