using System.Collections.Generic;

namespace Statescript.Compiler
{
   public class ActionAstNode : AstNode
   {
      public string Message { get; set; }
      public string Name { get; set; }
      public List<ParamAstNode> Params = new List<ParamAstNode>();
   }
}
