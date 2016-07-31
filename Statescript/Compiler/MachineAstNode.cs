using System.Collections.Generic;

namespace Statescript.Compiler
{
   public class MachineAstNode : AstNode
   {
      public string Name { get; set; }
      public ActionAstNode Action;
      public List<StateAstNode> States;
   }
}
