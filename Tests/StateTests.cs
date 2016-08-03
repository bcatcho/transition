using NUnit.Framework;
using Transition;
using System.Collections.Generic;

namespace Tests
{
   [TestFixture]
   public class StateTests
   {
      /// <summary>
      /// A helper class to produce actions with specific results
      /// </summary>
      private class TestAction : Action
      {
         public TickResult Result;

         public System.Action TickFunc;

         public TestAction(TickResult result)
         {
            Result = result;
         }

         public TestAction(TickResult result, System.Action tickFunc)
         {
            Result = result;
            TickFunc = tickFunc;
         }

         protected override TickResult OnTick(Context context)
         {
            if (TickFunc != null) {
               TickFunc.Invoke();
            }
            return Result;
         }
      }

      [Test]
      public void Tick_NoActions_ReturnsYield()
      {
         var state = new State();
         var context = new Context();

         var result = state.Tick(context);

         Assert.AreEqual(TickResultType.Yield, result.ResultType);
      }

      [Test]
      public void Tick_HasOneRunActionThatYeilds_ReturnsYield()
      {
         var state = new State();
         state.AddRunAction(new TestAction(TickResult.Yield()));
         var context = new Context();
         context.ExecState.ActionIndex = 0;

         var result = state.Tick(context);

         Assert.AreEqual(TickResultType.Yield, result.ResultType);
      }

      [Test]
      public void Tick_HasTwoRunActions_CorrectActionIsRun()
      {
         bool actionWasRun = false;
         var state = new State();
         state.AddRunAction(new TestAction(TickResult.Yield()));
         state.AddRunAction(new TestAction(TickResult.Done(), () => actionWasRun = true ));
         var context = new Context();
         context.ExecState.ActionIndex = 1;

         state.Tick(context);

         Assert.IsTrue(actionWasRun);
      }

      [Test]
      public void Tick_HasTwoRunActionsAndStartsAtFirstAndBothFinish_ResultIsYield()
      {
         var actionThatRan = new List<string>();
         var state = new State();
         state.AddRunAction(new TestAction(TickResult.Done(), () => actionThatRan.Add("first") ));
         state.AddRunAction(new TestAction(TickResult.Done(), () => actionThatRan.Add("second") ));
         var context = new Context();
         context.ExecState.ActionIndex = 0;

         state.Tick(context);

         Assert.AreEqual("first",  actionThatRan[0]);
         Assert.AreEqual("second",  actionThatRan[1]);
      }

      [Test]
      public void Tick_HasTwoRunActionsAndStartsAtFirstAndBothFinish_ActionIndexIsSetProperly()
      {
         var state = new State();
         state.AddRunAction(new TestAction(TickResult.Done()));
         state.AddRunAction(new TestAction(TickResult.Done()));
         var context = new Context();
         context.ExecState.ActionIndex = 0;

         state.Tick(context);

         Assert.AreEqual(2, context.ExecState.ActionIndex);
      }
   }
}
