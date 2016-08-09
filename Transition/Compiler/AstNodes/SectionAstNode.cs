using System.Collections.Generic;

namespace Transition.Compiler.AstNodes
{
   /// <summary>
   /// The Section node holds an ordered sequence of actions to be executed.
   /// It is used by the State node to encapsulate the actions in the On, Start, Exit, and Run sections
   /// </summary>
   public class SectionAstNode : AstNode
   {
      /// <summary>
      /// An ordered sequence of actions in this section.
      /// </summary>
      public List<ActionAstNode> Actions = new List<ActionAstNode>();
   }
}
