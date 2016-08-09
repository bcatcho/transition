using System;

namespace Transition.Compiler.ValueConverters
{
   /// <summary>
   /// Converts a state machine value into a string
   /// </summary>
   public class IntValueConverter : IValueConverter
   {
      public Type GetConverterType()
      {
         return typeof(int);
      }

      public bool TryConvert(string input, out object result)
      {
         int parsedInt;
         var parseWasSuccessful = int.TryParse(input, out parsedInt);
         result = parsedInt;
         return parseWasSuccessful;
      }
   }
}