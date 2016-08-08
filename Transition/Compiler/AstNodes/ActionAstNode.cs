using System.Collections.Generic;

namespace Transition.Compiler.AstNode
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

      private string _identifier;

      /// <summary>
      /// The unique ID (within a machine) of the state
      /// </summary>
      public string Identifier
      { 
         get { return _identifier; }
         // lowercase the identifier to reduce equality issues
         set { _identifier = value.ToLower(); }
      }


      /// <summary>
      /// All of the actions parameters in order.
      /// </summary>
      public List<ParamAstNode> Params = new List<ParamAstNode>();
   }
}
