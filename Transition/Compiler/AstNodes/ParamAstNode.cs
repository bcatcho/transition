namespace Transition.Compiler.AstNodes
{
   /// <summary>
   /// The Parameter node represents a name value pair that may be found in actions.
   /// The operator is important as it determines how to interpret the value.
   /// </summary>
   public class ParamAstNode : AstNode
   {
      private string _name;

      /// <summary>
      /// The unique name (within an action) of the parameter
      /// </summary>
      public string Name
      { 
         get { return _name; }
         // lowercase to reduce equality issues
         set { _name = value.ToLower(); }
      }

      /// <summary>
      /// The type of operation. This will change the type of the value at execution time.
      /// </summary>
      public ParamOperation Op { get; set; }

      /// <summary>
      /// The value of the parameter. Will be converted to a specific type at compile time.
      /// </summary>
      public string Val { get; set; }

      /// <summary>
      /// The value of the parameter if it was a transition parameter. This will be the StateId to transition to.
      /// It is set by the SymanticAnalyzer
      /// </summary>
      public int StateIdVal { get; set; }
   }
}
