using NUnit.Framework;
using Transition;
using System.Collections.Generic;

namespace Tests
{
   [TestFixture]
   public class StateTests
   {
      // to reduce boilerplate
      private Context _context;
      private State _state;

      [SetUp]
      public void SetUp()
      {
         _context = new Context();
         _state = new State();
      }

      [Test]
      public void Tick_NoActions_ReturnsYield()
      {
         var result = _state.Tick(_context);

         Assert.AreEqual(TickResultType.Yield, result.ResultType);
      }

      [Test]
      public void Tick_HasOneRunActionThatYeilds_ReturnsYield()
      {
         _state.AddRunAction(new TestAction(TickResult.Yield()));
         _context.ActionIndex = 0;

         var result = _state.Tick(_context);

         Assert.AreEqual(TickResultType.Yield, result.ResultType);
      }

      [Test]
      public void Tick_HasTwoRunActions_CorrectActionIsRun()
      {
         bool actionWasRun = false;
         _state.AddRunAction(new TestAction(TickResult.Yield()));
         _state.AddRunAction(new TestAction(TickResult.Done(), () => actionWasRun = true));
         _context.ActionIndex = 1;

         _state.Tick(_context);

         Assert.IsTrue(actionWasRun);
      }

      [Test]
      public void Tick_HasTwoRunActionsAndStartsAtFirstAndBothFinish_BothAreRunInOrder()
      {
         var actionThatRan = new List<string>();
         _state.AddRunAction(new TestAction(TickResult.Done(), () => actionThatRan.Add("first")));
         _state.AddRunAction(new TestAction(TickResult.Done(), () => actionThatRan.Add("second")));
         _context.ActionIndex = 0;

         _state.Tick(_context);

         Assert.AreEqual("first", actionThatRan[0]);
         Assert.AreEqual("second", actionThatRan[1]);
      }

      [Test]
      public void Tick_HasTwoRunActionsAndStartsAtFirstAndFirstYields_OnlyFirstIsRun()
      {
         var actionThatRan = new List<string>();
         _state.AddRunAction(new TestAction(TickResult.Yield(), () => actionThatRan.Add("first")));
         _state.AddRunAction(new TestAction(TickResult.Done(), () => actionThatRan.Add("second")));
         _context.ActionIndex = 0;

         _state.Tick(_context);

         Assert.AreEqual("first", actionThatRan[0]);
         Assert.AreEqual(1, actionThatRan.Count);
         Assert.AreEqual(0, _context.ActionIndex);
      }

      [Test]
      public void Tick_HasTwoRunActionsAndStartsAtFirstAndBothFinish_ActionIndexIsSetProperly()
      {
         _state.AddRunAction(new TestAction(TickResult.Done()));
         _state.AddRunAction(new TestAction(TickResult.Done()));
         _context.ActionIndex = 0;

         _state.Tick(_context);

         Assert.AreEqual(2, _context.ActionIndex);
      }

      [Test]
      public void Tick_HasTwoRunActionsAndSecondReturnsLoop_ActionIndexIsResetAndYieldReturned()
      {
         _state.AddRunAction(new TestAction(TickResult.Done()));
         _state.AddRunAction(new TestAction(TickResult.Loop()));
         _context.ActionIndex = 1;

         var result = _state.Tick(_context);

         Assert.AreEqual(0, _context.ActionIndex);
         Assert.AreEqual(TickResultType.Yield, result.ResultType);
      }

      [Test]
      public void Tick_ActionReturnsTransition_StateReturnsTransitionResult()
      {
         _state.AddRunAction(new TestAction(TickResult.Transition(3)));
         _context.ActionIndex = 0;

         var result = _state.Tick(_context);

         Assert.AreEqual(TickResultType.Transition, result.ResultType);
         Assert.AreEqual(3, result.TransitionId);
      }

      [Test]
      public void Enter_HasOneAction_ActionIsRun()
      {
         var actionThatRan = new List<string>();
         _state.AddEnterAction(new TestAction(TickResult.Done(), () => actionThatRan.Add("first")));

         _state.Enter(_context);

         Assert.AreEqual("first", actionThatRan[0]);
      }

      [Test]
      public void Enter_ActionIndexIsThree_ResetsActionIndex()
      {
         _context.ActionIndex = 3;

         _state.Enter(_context);

         Assert.AreEqual(0, _context.ActionIndex);
      }

      [Test]
      public void Enter_HasTwoActions_BothAreRunInOrder()
      {
         var actionThatRan = new List<string>();
         _state.AddEnterAction(new TestAction(TickResult.Done(), () => actionThatRan.Add("first")));
         _state.AddEnterAction(new TestAction(TickResult.Done(), () => actionThatRan.Add("second")));

         _state.Enter(_context);

         Assert.AreEqual("first", actionThatRan[0]);
         Assert.AreEqual("second", actionThatRan[1]);
      }

      [Test]
      [TestCase(TickResultType.Yield)]
      [TestCase(TickResultType.Loop)]
      [TestCase(TickResultType.Transition)]
      public void Enter_ActionDoesNotReturnYield_ErrorCodeRaised(TickResultType resultType)
      {
         _state.AddEnterAction(new TestAction(new TickResult { ResultType = resultType, TransitionId = 0 }));

         _state.Enter(_context);

         Assert.AreEqual(_context.LastError, ErrorCode.Exec_State_Enter_ActionDidNotReturnYield);
      }

      [Test]
      public void Exit_HasOneAction_ActionIsRun()
      {
         var actionThatRan = new List<string>();
         _state.AddExitAction(new TestAction(TickResult.Done(), () => actionThatRan.Add("first")));

         _state.Exit(_context);

         Assert.AreEqual("first", actionThatRan[0]);
      }

      [Test]
      public void Exit_HasTwoActions_BothAreRunInOrder()
      {
         var actionThatRan = new List<string>();
         _state.AddExitAction(new TestAction(TickResult.Done(), () => actionThatRan.Add("first")));
         _state.AddExitAction(new TestAction(TickResult.Done(), () => actionThatRan.Add("second")));

         _state.Exit(_context);

         Assert.AreEqual("first", actionThatRan[0]);
         Assert.AreEqual("second", actionThatRan[1]);
      }

      [Test]
      [TestCase(TickResultType.Yield)]
      [TestCase(TickResultType.Loop)]
      [TestCase(TickResultType.Transition)]
      public void Exit_ActionDoesNotReturnYield_ErrorCodeRaised(TickResultType resultType)
      {
         _state.AddExitAction(new TestAction(new TickResult { ResultType = resultType, TransitionId = 0 }));

         _state.Exit(_context);

         Assert.AreEqual(_context.LastError, ErrorCode.Exec_State_Exit_ActionDidNotReturnYield);
      }

      [Test]
      public void SendMessage_CanHandleMessage_MessageHandlerTicked()
      {
         var messageHandlerTicked = false;
         var action = new TestAction(TickResult.Done(), () => messageHandlerTicked = true);
         _state.AddOnAction("test", action);
         var message = new MessageEnvelope
         {
            Key = "test"
         };

         _state.SendMessage(_context, message);

         Assert.IsTrue(messageHandlerTicked);
      }

      [Test]
      public void SendMessage_CanHandleMessage_MessageIsClearedAfterRun()
      {
         var action = new TestAction(TickResult.Done());
         _state.AddOnAction("test", action);
         var message = new MessageEnvelope
         {
            Key = "test"
         };

         _state.SendMessage(_context, message);

         Assert.IsNull(_context.Message);
      }

      [Test]
      public void SendMessage_HandlerReturnsTransition_StateReturnsTransition()
      {
         var action = new TestAction(TickResult.Transition(1));
         _state.AddOnAction("test", action);
         var message = new MessageEnvelope
         {
            Key = "test"
         };

         var result = _state.SendMessage(_context, message);

         Assert.AreEqual(TickResultType.Transition, result.ResultType);
      }

      [Test]
      public void SendMessage_HandlerReturnsDone_StateReturnsDone()
      {
         var action = new TestAction(TickResult.Done());
         _state.AddOnAction("test", action);
         var message = new MessageEnvelope
         {
            Key = "test"
         };

         var result = _state.SendMessage(_context, message);

         Assert.AreEqual(TickResultType.Done, result.ResultType);
      }

      [Test]
      [TestCase(TickResultType.Loop)]
      [TestCase(TickResultType.Yield)]
      public void SendMessage_HandlerReturnsInvalidResult_StateReturnsDoneAndRaisesError(TickResultType invalideType)
      {
         var invalidResult = new TickResult
         {
            ResultType = invalideType
         };
         var action = new TestAction(invalidResult);
         _state.AddOnAction("test", action);
         var message = new MessageEnvelope
         {
            Key = "test"
         };

         var result = _state.SendMessage(_context, message);

         Assert.AreEqual(TickResultType.Done, result.ResultType);
         Assert.AreEqual(ErrorCode.Exec_State_SendMessage_MessageHandlerDidNotReturnTransitionOrDone, _context.LastError);
      }

      [Test]
      public void SendMessage_DoesNotHandleMessage_StateReturnsDone()
      {
         var message = new MessageEnvelope
         {
            Key = "test"
         };

         var result = _state.SendMessage(_context, message);

         Assert.AreEqual(TickResultType.Done, result.ResultType);
      }
   }
}
