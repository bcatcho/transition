namespace Transition
{
   /// <summary>
   /// A class that wraps value types. This is meant to reduce boxing when storing value types (like floats)
   /// in the Blackboard. It is highly recommended that all value types are wrapped in this or similar
   /// reference types.
   /// </summary>
   public class ValueWrapper<T>
   {
      public T Value { get; set; }

      public ValueWrapper()
      {
      }

      public ValueWrapper(T initialValue)
      {
         Value = initialValue;
      }
   }
}
