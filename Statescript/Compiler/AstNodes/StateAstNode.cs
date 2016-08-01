namespace Statescript.Compiler.AstNode
{
   public class StateAstNode : AstNode
   {
      public string Name;
      public SectionAstNode Enter;
      public SectionAstNode Exit;
      public SectionAstNode Run;
      public SectionAstNode On;
   }
}
