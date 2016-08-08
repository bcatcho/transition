using NUnit.Framework;
using Transition;
using System.Collections.Generic;

namespace Tests
{
   /// <summary>
   /// A helper class to produce actions with specific results
   /// </summary>
   internal class TestAction : Action<TestMachineContext>
   {
      public TickResult Result;

      public System.Action TickFunc;

      public TestAction()
      {
      }

      public TestAction(TickResult result)
      {
         Result = result;
      }

      public TestAction(TickResult result, System.Action tickFunc)
      {
         Result = result;
         TickFunc = tickFunc;
      }

      protected override TickResult OnTick(TestMachineContext context)
      {
         if (TickFunc != null) {
            TickFunc.Invoke();
         }
         return Result;
      }
   }
   
}
