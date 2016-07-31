namespace Statescript.Compiler
{
   public class StateAstNode : AstNode
   {
      public SectionAstNode Enter;
      public SectionAstNode Exit;
      public SectionAstNode Run;
      public SectionAstNode On;
   }
}
