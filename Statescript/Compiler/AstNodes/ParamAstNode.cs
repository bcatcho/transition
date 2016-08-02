namespace Statescript.Compiler.AstNode
{
   /// <summary>
   /// The Parameter node represents a name value pair that may be found in actions.
   /// The operator is important as it determines how to interpret the value.
   /// </summary>
   public class ParamAstNode : AstNode
   {
      /// <summary>
      /// The name of the parameter. Used to supply the right value to an action.
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// The type of operation. This will change the type of the value at execution time.
      /// </summary>
      public ParamOperation Op { get; set; }

      /// <summary>
      /// The value of the parameter. Will be converted to a specific type at compile time.
      /// </summary>
      public string Val { get; set; }
   }
}
