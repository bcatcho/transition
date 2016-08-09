namespace Transition.Compiler.AstNodes
{
   /// <summary>
   /// Base class for all Abstract Syntax Tree Nodes
   /// </summary>
   public class AstNode
   {
      /// <summary>
      /// The line number of the token that produced this node
      /// </summary>
      public int LineNumber { get; set; }
   }
}
