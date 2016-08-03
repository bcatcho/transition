using System.Collections.Generic;

namespace Transition
{
   public class Context
   {
      public ErrorCode LastError { get; private set; }

      public ExecutionState ExecState { get; private set; }

      public Context()
      {
         ExecState = new ExecutionState();
      }

      public void ResetError()
      {
         LastError = ErrorCode.None;
      }

      public void RaiseError(ErrorCode code)
      {
         LastError = code;
      }
   }
}
