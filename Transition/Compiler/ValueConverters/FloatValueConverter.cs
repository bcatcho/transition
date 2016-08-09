using System;

namespace Transition.Compiler.ValueConverters
{
   /// <summary>
   /// Converts a state machine value into a float
   /// </summary>
   public class FloatValueConverter : IValueConverter
   {
      public Type GetConverterType()
      {
         return typeof(float);
      }

      public bool TryConvert(string input, out object result)
      {
         float parsedFloat;
         var parseWasSuccessful = float.TryParse(input, out parsedFloat);
         result = parsedFloat;
         return parseWasSuccessful;
      }
   }
}