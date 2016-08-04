using NUnit.Framework;
using Transition;
using System.Collections.Generic;

namespace Tests
{
   [TestFixture]
   public class MachineTests
   {
      private Machine _machine;
      private Context _context;

      [TestFixtureSetUp]
      public void SetUp()
      {
         _machine = new Machine();
         _context = new Context();
      }

      private State NoopState()
      {
         var state = new State();
         return state;
      }

      [Test]
      public void Tick_Uninitialized_TransitionsToFirstState()
      {
         _machine.AddState(NoopState());
         _machine.EnterAction = new TestAction(TickResult.Transition(0));

         _machine.Tick(_context);

         Assert.AreEqual(0, _context.ExecState.StateId);
      }
   }
}
