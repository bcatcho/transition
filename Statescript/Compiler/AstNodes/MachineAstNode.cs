using System.Collections.Generic;

namespace Statescript.Compiler.AstNode
{
   /// <summary>
   /// The Machine node is the root node for the Abstract Syntax Tree.
   /// It consists of all the states found in the same file.
   /// It must have a name and an action. The action must transition to the initial state.
   /// </summary>
   public class MachineAstNode : AstNode
   {
      /// <summary>
      /// The unique name of the Machine
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// An action that must transition the Machine to the initial state.
      /// </summary>
      public ActionAstNode Action;

      /// <summary>
      /// A list of all the states in the Machine. Must not be empty.
      /// </summary>
      public List<StateAstNode> States = new List<StateAstNode>();
   }
}
