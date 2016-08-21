using NUnit.Framework;
using Transition;
using System.Collections.Generic;

namespace Tests
{
   [TestFixture]
   public class MachineControllerTests
   {
      private MachineController<Context> _controller;
      private List<Context> _contexts;

      public class Increment : Action<Context>
      {
         protected override TickResult OnTick(Context context)
         {
            var val = context.Blackboard.Get<int>("x");
            context.Blackboard.Set<int>("x", val + 1);
            return TickResult.Yield();
         }
      }

      [SetUp]
      public void SetUp()
      {
         _contexts = new List<Context>();
         _controller = new DefaultMachineController();
         _controller.LoadActions(typeof(Increment));
         _controller.Compile("@machine a -> b\n@state b\n@run\nincrement");
      }

      [Test]
      public void TickAll_MachineExists_MachineIsTicked()
      {
         _contexts.Add(_controller.AddMachineInstance("a"));
         _contexts[0].Blackboard.Set<int>("x", 0);

         // states will enter (nothing will happen)
         _controller.TickAll();
         // states will run
         _controller.TickAll();

         Assert.AreEqual(1, _contexts[0].Blackboard.Get<int>("x"));
      }

      [Test]
      public void TickAll_TwoMachinesExists_BothMachinesTicked()
      {
         _contexts.Add(_controller.AddMachineInstance("a"));
         _contexts[0].Blackboard.Set<int>("x", 0);
         _contexts.Add(_controller.AddMachineInstance("a"));
         _contexts[1].Blackboard.Set<int>("x", 0);

         // states will enter (nothing will happen)
         _controller.TickAll();
         // states will run
         _controller.TickAll();

         Assert.AreEqual(1, _contexts[0].Blackboard.Get<int>("x"));
         Assert.AreEqual(1, _contexts[1].Blackboard.Get<int>("x"));
      }

      [Test]
      public void RemoveMachineInstance_RemoveFirstOfTwo_OnlySecondMachineTicked()
      {
         _contexts.Add(_controller.AddMachineInstance("a"));
         _contexts[0].Blackboard.Set<int>("x", 0);
         _contexts.Add(_controller.AddMachineInstance("a"));
         _contexts[1].Blackboard.Set<int>("x", 0);

         _controller.RemoveMachineInstance(_contexts[0].ContextId);
         // states will enter (nothing will happen)
         _controller.TickAll();
         // states will run
         _controller.TickAll();

         // ensure the context will have been cleared
         Assert.IsFalse(_contexts[0].Blackboard.Exists("x"));
         Assert.AreEqual(1, _contexts[1].Blackboard.Get<int>("x"));
      }

      [Test]
      public void RemoveMachineInstance_MachineAddedAfterRemove_BothTicked()
      {
         _contexts.Add(_controller.AddMachineInstance("a"));
         _contexts[0].Blackboard.Set<int>("x", 0);
         _contexts.Add(_controller.AddMachineInstance("a"));
         _contexts[1].Blackboard.Set<int>("x", 0);

         _controller.RemoveMachineInstance(_contexts[0].ContextId);
         _contexts.Add(_controller.AddMachineInstance("a"));
         _contexts[2].Blackboard.Set<int>("x", 0);
         // states will enter (nothing will happen)
         _controller.TickAll();
         // states will run
         _controller.TickAll();

         // ensure the context will have been cleared
         Assert.AreEqual(1, _contexts[1].Blackboard.Get<int>("x"));
         Assert.AreEqual(1, _contexts[2].Blackboard.Get<int>("x"));
      }

      [Test]
      public void Tick_MachineInstanceExists_MachineIsTicked()
      {
         _contexts.Add(_controller.AddMachineInstance("a"));
         _contexts[0].Blackboard.Set<int>("x", 0);

         // states will enter (nothing will happen)
         _controller.Tick(_contexts[0].ContextId);
         // states will run
         _controller.Tick(_contexts[0].ContextId);

         Assert.AreEqual(1, _contexts[0].Blackboard.Get<int>("x"));
      }

      [Test]
      public void LoadActions_OneActionLoaded_IsExecuted()
      {
         _controller = new DefaultMachineController();

         _controller.LoadActions(typeof(Increment));
         _controller.Compile("@machine a -> b\n@state b\n@enter\nincrement");
         _contexts.Add(_controller.AddMachineInstance("a"));
         _contexts[0].Blackboard.Set<int>("x", 0);
         _controller.TickAll();

         Assert.AreEqual(1, _contexts[0].Blackboard.Get<int>("x"));
      }
   }
}
