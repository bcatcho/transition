using NUnit.Framework;
using Transition.Compiler;
using Transition.Compiler.AstNode;
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
         _generator = new MachineGenerator<TestMachineContext>();
         _generator.Initialize();
      }

      [TestCase]
      public void Generate_MachineWithId_IdIsSet()
      {
         _machineNode.Identifier = "mach";
         _machineNode.Action = new ActionAstNode()
         {
            Identifier = ParserConstants.TransitionAction
         };

         var result = _generator.Generate(_machineNode);

         Assert.AreEqual("mach", result.Identifier);
      }

      [TestCase]
      public void Generate_MachineWithAction_ActionIsTransition()
      {
         _machineNode.Identifier = "mach";
         _machineNode.Action = new ActionAstNode()
         {
            Identifier = ParserConstants.TransitionAction
         };

         var result = _generator.Generate(_machineNode);

         Assert.IsInstanceOf<TransitionAction<TestMachineContext>>(result.EnterAction);
      }
   }
}