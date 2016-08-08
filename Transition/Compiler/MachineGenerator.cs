using Transition.Compiler.AstNode;

namespace Transition.Compiler
{
   /// <summary>
   /// The "code" generator for the compiler. Accepts a validated syntax tree and returns an executable Machine.
   /// </summary>
   public class MachineGenerator
   {
      /// <summary>
      /// Generates an executable Machine from a syntax tree
      /// </summary>
      /// <param name="machineAst">Machine syntax tree node</param>
      public Machine Generate(MachineAstNode machineAst)
      {
         var machine = new Machine
         {
            Identifier = machineAst.Identifier,
         };

         return machine;
      }
   }
}
