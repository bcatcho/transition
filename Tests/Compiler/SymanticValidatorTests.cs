using System;
using NUnit.Framework;
using Transition.Compiler;
using Transition.Compiler.AstNode;
using Transition;

namespace Tests.Compiler
{
   [TestFixture]
   public class SymanticValidatorTests
   {
      [Test]
      public void IsValid_StatesForTransitionsExist_ReturnsTrue()
      {
         var machine = new MachineAstNode();
         var state1 = new StateAstNode() {
            Identifier = "state1"
         };
         var action1 = new ActionAstNode();
         action1.Params.Add( new ParamAstNode {
            Op = ParamOperation.Transition,
            Val = "state1"
         });
         state1.Run = new SectionAstNode();
         state1.Run.Actions.Add(action1);
         machine.States.Add(state1);
         SymanticValidator validator = new SymanticValidator();

         ErrorCode errorCode;
         var result = validator.IsValid(machine, out errorCode);

         Assert.IsTrue(result);
      }

      [Test]
      public void ValidateTransitions_StatesForTransitionsAreMissing_ReturnsFalseAndErrorCode()
      {
         var machine = new MachineAstNode();
         var state1 = new StateAstNode() {
            Identifier = "state1"
         };
         var action1 = new ActionAstNode();
         action1.Params.Add( new ParamAstNode {
            Op = ParamOperation.Transition,
            Val = "NOT_STATE_1"
         });
         state1.Run = new SectionAstNode();
         state1.Run.Actions.Add(action1);
         machine.States.Add(state1);
         SymanticValidator validator = new SymanticValidator();

         ErrorCode errorCode;
         var result = validator.IsValid(machine, out errorCode);

         Assert.IsFalse(result);
         Assert.AreEqual(ErrorCode.Validate_TransitionParams_StateNotFoundForTransition, errorCode);
      }
   }
}
