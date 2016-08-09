using System;

namespace Transition.Compiler.ValueConverters
{
   /// <summary>
   /// Converts a state machine value into a string
   /// </summary>
   public class StringValueConverter : IValueConverter
   {
      public Type GetConverterType()
      {
         return typeof(string);
      }

      public bool TryConvert(string input, out object result)
      {
         result = input;
         return true;
      }
   }
}