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

         var action = CurrentAction(context);

         // no action == noop
         if (action == null) {
            return TickResult.Yield();
         }

         return action.Tick(context);
      }

      private Action CurrentAction(Context context)
      {
         if (context.ExecState.ActionIndex < RunActions.Count) {
            return RunActions[context.ExecState.ActionIndex];
         }

         return null;
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
