namespace Statescript.Compiler.AstNode
{
   public class ParamAstNode : AstNode
   {
      public string Name { get; set; }
      public ParamOperation Op { get; set; }
      public string Val { get; set; }
   }
}
