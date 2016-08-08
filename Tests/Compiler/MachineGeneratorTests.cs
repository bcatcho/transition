using NUnit.Framework;
using Transition.Compiler;
using Transition.Compiler.AstNode;

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
      }

      [TestCase]
      public void Generate_MachineWithId_IdIsSet()
      {
         _machineNode.Identifier = "mach";

         var result = _generator.Generate(_machineNode);

         Assert.AreEqual("mach", result.Identifier);
      }
   }
}