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
               case TickResultType.Done:
                  // if the last action finished, advance
                  action = AdvanceAction(context);
                  break;
               case TickResultType.Yield:
                  return TickResult.Yield();
               case TickResultType.Loop:
                  ResetForLooping(context);
                  return TickResult.Yield();
               default:
                  return TickResult.Yield();
            }
         }

         return TickResult.Yield();
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
   }
}
