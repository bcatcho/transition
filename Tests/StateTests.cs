using NUnit.Framework;
using Transition;

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

         public TestAction(TickResult result)
         {
            Result = result;
         }

         protected override TickResult OnTick(Context context)
         {
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
      public void Tick_HasOneRunAction_ReturnsActionsResult()
      {
         var state = new State();
         state.AddRunAction(new TestAction(TickResult.Done()));
         var context = new Context();

         var result = state.Tick(context);

         Assert.AreEqual(TickResultType.Done, result.ResultType);
      }
   }
}

