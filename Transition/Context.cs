using System.Collections.Generic;

namespace Transition
{

   public class Context
   {
      public ExecutionState ExecState { get; private set; }

      public Context()
      {
         ExecState = new ExecutionState();
      }
   }
   
}
