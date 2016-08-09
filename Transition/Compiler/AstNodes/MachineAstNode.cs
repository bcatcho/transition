using System.Collections.Generic;

namespace Transition.Compiler.AstNodes
{
   /// <summary>
   /// The Machine node is the root node for the Abstract Syntax Tree.
   /// It consists of all the states found in the same file.
   /// It must have a name and an action. The action must transition to the initial state.
   /// </summary>
   public class MachineAstNode : AstNode
   {
      private string _identifier;

      /// <summary>
      /// The unique ID (or name) of the Machine
      /// </summary>
      public string Identifier
      { 
         get { return _identifier; }
         // lowercase the identifier to reduce equality issues
         set { _identifier = value.ToLower(); }
      }

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
