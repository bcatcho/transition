using NUnit.Framework;
using Transition;

namespace Tests
{
   [TestFixture]
   public class MachineTests
   {
      // to reduce boilerplate
      private Machine _machine;
      private Context _context;

      [SetUp]
      public void SetUp()
      {
         // be sure to use a new machine and context for each step
         _machine = new Machine();
         _context = new Context();
      }

      [Test]
      public void Tick_FirstTick_TransitionsToFirstState()
      {
         _machine.AddState(new State());
         _machine.EnterAction = new TestAction(TickResult.Transition(0));

         _machine.Tick(_context);

         Assert.AreEqual(0, _context.ExecState.StateId);
      }

      [Test]
      public void Tick_FirstTick_FirstStateIsEntered()
      {
         var state = new State();
         // add a state that will flip a flat when entered
         var firstStateEntered = false;
         state.AddEnterAction(new TestAction(TickResult.Done(), () => firstStateEntered = true));
         _machine.AddState(state);
         _machine.EnterAction = new TestAction(TickResult.Transition(0));

         _machine.Tick(_context);

         Assert.IsTrue(firstStateEntered);
      }

      [Test]
      public void Tick_EnterActionReturnsBadResult_RaisesError()
      {
         _machine.EnterAction = new TestAction(TickResult.Done());

         _machine.Tick(_context);

         Assert.AreEqual(ErrorCode.Exec_Machine_Tick_MachineActionMustReturnTransition, _context.LastError);
      }

      [Test]
      public void Tick_CurrentStateDoesNotExist_RaisesError()
      {
         _machine.EnterAction = new TestAction(TickResult.Done());

         _machine.Tick(_context);

         Assert.AreEqual(ErrorCode.Exec_Machine_Tick_MachineActionMustReturnTransition, _context.LastError);
      }

      [Test]
      public void Tick_TransitionToUnavailableState_RaisesError()
      {
         // add an action that will transition to a state that does not exist
         _machine.EnterAction = new TestAction(TickResult.Transition(2));

         _machine.Tick(_context);

         Assert.AreEqual(ErrorCode.Exec_Machine_Transition_DestinationStateDoesNotExist, _context.LastError);
      }

      [Test]
      public void Tick_TransitionFromState_FirstStateIsExitedSecondIsEntered()
      {
         var state0exited = false;
         var state1entered = false;
         // add an action that will transition to a state that does not exist
         var state0 = new State();
         state0.AddExitAction(new TestAction(TickResult.Done(), () => state0exited = true));
         state0.AddRunAction(new TestAction(TickResult.Transition(1)));
         _machine.AddState(state0);

         var state1 = new State();
         state1.AddEnterAction(new TestAction(TickResult.Done(), () => state1entered = true));
         _machine.AddState(state1);
         // make sure the execution state is set to run the first state's Run action
         _context.ExecState.ActionIndex = 0;
         _context.ExecState.StateId = 0;

         _machine.Tick(_context);

         Assert.IsTrue(state0exited);
         Assert.IsTrue(state1entered);
      }

      [Test]
      public void Tick_StateReturnsDone_ActiveStateIdRemainsTheSame()
      {
         var state0 = new State();
         state0.AddRunAction(new TestAction(TickResult.Done()));
         _machine.AddState(state0);

         // make sure the execution state is set to run the first state's Run action
         _context.ExecState.ActionIndex = 0;
         _context.ExecState.StateId = 0;

         _machine.Tick(_context);

         Assert.AreEqual(0, _context.ExecState.StateId);
      }

      [Test]
      public void SendMessage_StateReturnsTransition_TransitionOccurs()
      {
         var state0 = new State();
         state0.AddOnAction("test", new TestAction(TickResult.Transition(1)));
         _machine.AddState(state0);
         // add destination state
         _machine.AddState(new State());
         var message = new MessageEnvelope
         {
            Key = "test"
         };
         // make sure the execution state makes state0 the active state
         _context.ExecState.StateId = 0;

         _machine.SendMessage(_context, message);

         Assert.AreEqual(1, _context.ExecState.StateId);
      }

      [Test]
      public void SendMessage_StateReturnsDone_ExecStateRemainsTheSame()
      {
         var state0 = new State();
         state0.AddOnAction("test", new TestAction(TickResult.Done()));
         _machine.AddState(state0);
         var message = new MessageEnvelope
         {
            Key = "test"
         };
         // make sure the execution state makes state0 the active state
         _context.ExecState.StateId = 0;

         _machine.SendMessage(_context, message);

         Assert.AreEqual(0, _context.ExecState.StateId);
      }

      [Test]
      public void SendMessage_NoStateIsAvailable_ExecStateRemainsTheSame()
      {
         var message = new MessageEnvelope
         {
            Key = "test"
         };
         // make sure the execution state makes state0 the active state
         _context.ExecState.StateId = 0;

         _machine.SendMessage(_context, message);

         Assert.AreEqual(0, _context.ExecState.StateId);
      }

      [Test]
      public void Tick_HasPreviousError_ResetsError()
      {
         _machine.AddState(new State());
         _context.ExecState.StateId = 0;
         _context.LastError = ErrorCode.Exec_Machine_Transition_DestinationStateDoesNotExist;

         _machine.Tick(_context);

         Assert.AreEqual(ErrorCode.None, _context.LastError);
      }
   }
}
