using System;

namespace Transition.Compiler.ValueConverters
{
   /// <summary>
   /// Implement this interface to convert a string in a state machine into a specific type.
   /// </summary>
   public interface IValueConverter
   {
      Type GetConverterType();

      bool TryConvert(string input, out object result);
   }
}