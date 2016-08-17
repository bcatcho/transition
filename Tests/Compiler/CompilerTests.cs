using NUnit.Framework;
using Transition.Compiler;
using Transition.Actions;

namespace Tests.Compiler
{
   [TestFixture]
   public class MachineCompilerTests
   {
      private MachineCompiler<TestMachineContext> _compiler;

      [SetUp]
      public void SetUp()
      {
         _compiler = new MachineCompiler<TestMachineContext>();
         _compiler.LoadActions(typeof(TestAction));
      }

      [Test]
      public void Compile_StateWithAction_AllNodesAreCompiled()
      {
         var input = "@machine m -> 'state1'\n@state state1\n\t@on\n\t'msg': TestAction";

         var result = _compiler.Compile(input);

         Assert.AreEqual("m", result.Identifier);
         Assert.AreEqual(0, ((TransitionAction<TestMachineContext>)result.EnterAction).Destination.StateId);
         Assert.AreEqual("state1", result.States[0].Identifier);
         Assert.IsInstanceOf<TestAction>(result.States[0].OnActions["msg"]);
      }
   }
}