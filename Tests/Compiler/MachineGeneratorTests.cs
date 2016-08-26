using NUnit.Framework;
using Transition.Compiler;
using Transition.Compiler.AstNodes;
using Transition.Actions;

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
         _machineNode.Name = "mach";
         _machineNode.Action = new ActionAstNode() { Name = ParserConstants.TransitionAction };
         _generator = new MachineGenerator<TestMachineContext>();
         _generator.LoadActions(typeof(TestAction));
      }

      [TestCase]
      public void Generate_MachineWithId_IdIsSet()
      {
         _machineNode.Name = "mach2";

         var result = _generator.Generate(_machineNode);

         Assert.AreEqual("mach2", result.Name);
      }

      [TestCase]
      public void Generate_MachineWithAction_ActionIsTransition()
      {
         _machineNode.Action = new ActionAstNode() { Name = ParserConstants.TransitionAction };

         var result = _generator.Generate(_machineNode);

         Assert.IsInstanceOf<TransitionAction<TestMachineContext>>(result.EnterAction);
      }

      [TestCase]
      public void Generate_StateExists_MachineHasState()
      {
         var state = new StateAstNode
         {
            Name = "state1"
         };
         _machineNode.States.Add(state);

         var result = _generator.Generate(_machineNode);

         Assert.AreEqual("state1", result.States[0].Name);
      }

      [TestCase]
      public void Generate_StateWithRunAction_ActionIsGenerated()
      {
         var state = new StateAstNode
         {
            Name = "state1"
         };
         state.Run = new SectionAstNode();
         state.Run.Actions.Add(new ActionAstNode
         {
            Name = "testaction",
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
            Name = "state1"
         };
         state.Run = new SectionAstNode();
         state.Run.Actions.Add(new ActionAstNode
         {
            Name = ParserConstants.TransitionAction,
         });
         state.Run.Actions.Add(new ActionAstNode
         {
            Name = "testaction",
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
            Name = "state1"
         };
         state.Enter = new SectionAstNode();
         state.Enter.Actions.Add(new ActionAstNode
         {
            Name = "testaction",
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
            Name = "state1"
         };
         state.Exit = new SectionAstNode();
         state.Exit.Actions.Add(new ActionAstNode
         {
            Name = "testaction",
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
            Name = "state1"
         };
         state.On = new SectionAstNode();
         state.On.Actions.Add(new ActionAstNode
         {
            Message = "blah",
            Name = "testaction"
         });
         _machineNode.States.Add(state);

         var result = _generator.Generate(_machineNode);

         Assert.IsInstanceOf<TestAction>(result.States[0].OnActions["blah"]);
      }

      [TestCase]
      public void Generate_ActionWithIntParameter_ParameterIsAssigned()
      {
         var state = new StateAstNode
         {
            Name = "state1"
         };
         state.On = new SectionAstNode();
         var action = new ActionAstNode
         {
            Message = "blah",
            Name = "testaction"
         };
         action.Params.Add(new ParamAstNode
         {
            Name = "TestProperty1",
            Op = ParamOperation.Assign,
            Val = "1234"
         });

         state.On.Actions.Add(action);
         _machineNode.States.Add(state);

         var result = _generator.Generate(_machineNode);

         var resultAction = (TestAction)result.States[0].OnActions["blah"];
         Assert.AreEqual(1234, resultAction.TestProperty1);
      }

      [TestCase]
      public void Generate_ActionWithTransitionParameter_ParameterIsTransitionDestination()
      {
         var state = new StateAstNode
         {
            Name = "state1"
         };
         state.On = new SectionAstNode();
         var action = new ActionAstNode
         {
            Message = "blah",
            Name = "testaction"
         };
         action.Params.Add(new ParamAstNode
         {
            Name = "DestinationProp",
            Op = ParamOperation.Transition,
            Val = "state2",
            StateIdVal = 1
         });
         state.On.Actions.Add(action);
         _machineNode.States.Add(state);
         // add another state to transition to
         _machineNode.States.Add(new StateAstNode
         {
            Name = "state2"
         });

         var result = _generator.Generate(_machineNode);

         var resultAction = (TestAction)result.States[0].OnActions["blah"];
         Assert.IsNotNull(resultAction.DestinationProp);
         Assert.AreEqual(1, resultAction.DestinationProp.StateId);
      }

      [TestCase]
      public void Generate_ActionWithTwoParameters_ParametersAreAssigned()
      {
         var state = new StateAstNode
         {
            Name = "state1"
         };
         state.On = new SectionAstNode();
         var action = new ActionAstNode
         {
            Message = "blah",
            Name = "testaction"
         };
         action.Params.Add(new ParamAstNode
         {
            Name = "TestProperty1",
            Op = ParamOperation.Assign,
            Val = "1234"
         });

         action.Params.Add(new ParamAstNode
         {
            Name = "TestProperty2",
            Op = ParamOperation.Assign,
            Val = "hello"
         });

         state.On.Actions.Add(action);
         _machineNode.States.Add(state);

         var result = _generator.Generate(_machineNode);

         var resultAction = (TestAction)result.States[0].OnActions["blah"];
         Assert.AreEqual(1234, resultAction.TestProperty1);
         Assert.AreEqual("hello", resultAction.TestProperty2);
      }

      [TestCase]
      public void Generate_ActionWithDefaultParameter_ParameterIsAssigned()
      {
         var state = new StateAstNode
         {
            Name = "state1"
         };
         state.On = new SectionAstNode();
         var action = new ActionAstNode
         {
            Message = "blah",
            Name = "TestAction"
         };
         action.Params.Add(new ParamAstNode
         {
            Name = ParserConstants.DefaultParameterName,
            Op = ParamOperation.Assign,
            Val = "1234"
         });

         state.On.Actions.Add(action);
         _machineNode.States.Add(state);

         var result = _generator.Generate(_machineNode);

         var resultAction = (TestAction)result.States[0].OnActions["blah"];
         Assert.AreEqual(1234, resultAction.TestProperty1);
      }
   }
}