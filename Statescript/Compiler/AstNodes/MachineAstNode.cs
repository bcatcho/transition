using System.Collections.Generic;

namespace Statescript.Compiler.AstNode
{
   public class MachineAstNode : AstNode
   {
      public string Name { get; set; }
      public ActionAstNode Action;
      public List<StateAstNode> States = new List<StateAstNode>();
   }
}
