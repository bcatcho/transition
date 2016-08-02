using System.Collections.Generic;

namespace Statescript.Compiler.AstNode
{
   public class SectionAstNode : AstNode
   {
      public List<ActionAstNode> Actions = new List<ActionAstNode>();
   }
}
