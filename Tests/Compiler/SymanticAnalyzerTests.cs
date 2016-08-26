using System;
using NUnit.Framework;
using Transition.Compiler;
using Transition.Compiler.AstNodes;
using Transition;

namespace Tests.Compiler
{
   [TestFixture]
   public class SymanticAnalyzerTests
   {
      [Test]
      public void Analyze_StatesForTransitionsExist_ReturnsTrue()
      {
         var machine = new MachineAstNode();
         var state1 = new StateAstNode() {
            Name = "state1"
         };
         var action1 = new ActionAstNode();
         action1.Params.Add( new ParamAstNode {
            Op = ParamOperation.Transition,
            Val = "state1"
         });
         state1.Run = new SectionAstNode();
         state1.Run.Actions.Add(action1);
         machine.States.Add(state1);
         SymanticAnalyzer validator = new SymanticAnalyzer();

         ErrorCode errorCode;
         var result = validator.Analyze(machine, out errorCode);

         Assert.IsTrue(result);
      }

      [Test]
      public void Analyze_StatesForTransitionsExist_StateIdValIsSet()
      {
         var machine = new MachineAstNode();
         var state1 = new StateAstNode() {
            Name = "state1"
         };
         var state2 = new StateAstNode() {
            Name = "state2"
         };
         var action1 = new ActionAstNode();
         action1.Params.Add( new ParamAstNode {
            Op = ParamOperation.Transition,
            Val = "state2"
         });
         state1.Run = new SectionAstNode();
         state1.Run.Actions.Add(action1);
         machine.States.Add(state1);
         machine.States.Add(state2);
         SymanticAnalyzer validator = new SymanticAnalyzer();

         ErrorCode errorCode;
         validator.Analyze(machine, out errorCode);

         Assert.AreEqual(1, machine.States[0].Run.Actions[0].Params[0].StateIdVal);
      }

      [Test]
      public void ValidateTransitions_StatesForTransitionsAreMissing_ReturnsFalseAndErrorCode()
      {
         var machine = new MachineAstNode();
         var state1 = new StateAstNode() {
            Name = "state1"
         };
         var action1 = new ActionAstNode();
         action1.Params.Add( new ParamAstNode {
            Op = ParamOperation.Transition,
            Val = "NOT_STATE_1"
         });
         state1.Run = new SectionAstNode();
         state1.Run.Actions.Add(action1);
         machine.States.Add(state1);
         SymanticAnalyzer validator = new SymanticAnalyzer();

         ErrorCode errorCode;
         var result = validator.Analyze(machine, out errorCode);

         Assert.IsFalse(result);
         Assert.AreEqual(ErrorCode.Validate_TransitionParams_StateNotFoundForTransition, errorCode);
      }
   }
}
