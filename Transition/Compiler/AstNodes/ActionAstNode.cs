using System.Collections.Generic;

namespace Transition.Compiler.AstNodes
{
   /// <summary>
   /// ActionAst node represents an action in a Section. It may or may not have params.
   /// If it is in the "On" section it must contain a message.
   /// </summary>
   public class ActionAstNode : AstNode
   {
      /// <summary>
      /// The name of the message if this action is a response to a message. May be null otherwise.
      /// </summary>
      public string Message { get; set; }

      private string _name;

      /// <summary>
      /// The name of the action
      /// </summary>
      public string Name
      { 
         get { return _name; }
         // lowercase the name to reduce equality issues
         set { _name = value.ToLower(); }
      }


      /// <summary>
      /// All of the actions parameters in order.
      /// </summary>
      public List<ParamAstNode> Params = new List<ParamAstNode>();
   }
}
