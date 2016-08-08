using NUnit.Framework;
using Transition.Compiler;
using Transition.Compiler.AstNode;
using Transition.Actions;
using System.Reflection;

namespace Tests.Compiler
{
   [TestFixture]
   public class MachineGeneratorTests
   {
      private MachineAstNode _machineNode;
      private MachineGenerator<TestMachineContext> _generator;

      [SetUp]
      public void SetUp()
      {
         _machineNode = new MachineAstNode();
         _machineNode.Identifier = "mach";
         _machineNode.Action = new ActionAstNode() { Identifier = ParserConstants.TransitionAction };
         _generator = new MachineGenerator<TestMachineContext>();
         _generator.Initialize(Assembly.GetAssembly(typeof(TestAction)));
      }

      [TestCase]
      public void Generate_MachineWithId_IdIsSet()
      {
         _machineNode.Identifier = "mach2";

         var result = _generator.Generate(_machineNode);

         Assert.AreEqual("mach2", result.Identifier);
      }

      [TestCase]
      public void Generate_MachineWithAction_ActionIsTransition()
      {
         _machineNode.Action = new ActionAstNode() { Identifier = ParserConstants.TransitionAction };

         var result = _generator.Generate(_machineNode);

         Assert.IsInstanceOf<TransitionAction<TestMachineContext>>(result.EnterAction);
      }


      [TestCase]
      public void Generate_StateExists_MachineHasState()
      {
         var state = new StateAstNode
         {
            Identifier = "state1"
         };
         _machineNode.States.Add(state);

         var result = _generator.Generate(_machineNode);

         Assert.AreEqual("state1", result.States[0].Identifier);
      }

      [TestCase]
      public void Generate_StateWithRunAction_ActionIsGenerated()
      {
         var state = new StateAstNode
         {
            Identifier = "state1"
         };
         state.Run = new SectionAstNode();
         state.Run.Actions.Add(new ActionAstNode
         {
            Identifier = "testaction",
         });
         _machineNode.States.Add(state);

         var result = _generator.Generate(_machineNode);

         Assert.IsInstanceOf<TestAction>(result.States[0].RunActions[0]);
      }

      [TestCase]
      public void Generate_StateWithTwoRunActions_ActionsAreGenerated()
      {
         var state = new StateAstNode
         {
            Identifier = "state1"
         };
         state.Run = new SectionAstNode();
         state.Run.Actions.Add(new ActionAstNode
         {
            Identifier = ParserConstants.TransitionAction,
         });
         state.Run.Actions.Add(new ActionAstNode
         {
            Identifier = "testaction",
         });
         _machineNode.States.Add(state);

         var result = _generator.Generate(_machineNode);

         Assert.IsInstanceOf<TestAction>(result.States[0].RunActions[1]);
      }

      [TestCase]
      public void Generate_StateWithEnterAction_ActionIsGenerated()
      {
         var state = new StateAstNode
         {
            Identifier = "state1"
         };
         state.Enter = new SectionAstNode();
         state.Enter.Actions.Add(new ActionAstNode
         {
            Identifier = "testaction",
         });
         _machineNode.States.Add(state);

         var result = _generator.Generate(_machineNode);

         Assert.IsInstanceOf<TestAction>(result.States[0].EnterActions[0]);
      }

      [TestCase]
      public void Generate_StateWithExitAction_ActionIsGenerated()
      {
         var state = new StateAstNode
         {
            Identifier = "state1"
         };
         state.Exit = new SectionAstNode();
         state.Exit.Actions.Add(new ActionAstNode
         {
            Identifier = "testaction",
         });
         _machineNode.States.Add(state);

         var result = _generator.Generate(_machineNode);

         Assert.IsInstanceOf<TestAction>(result.States[0].ExitActions[0]);
      }

      [TestCase]
      public void Generate_StateWithOnAction_ActionIsGenerated()
      {
         var state = new StateAstNode
         {
            Identifier = "state1"
         };
         state.On = new SectionAstNode();
         state.On.Actions.Add(new ActionAstNode
         {
            Message = "blah",
            Identifier = "testaction"
         });
         _machineNode.States.Add(state);

         var result = _generator.Generate(_machineNode);

         Assert.IsInstanceOf<TestAction>(result.States[0].OnActions["blah"]);
      }
   }
}