using System;
using NUnit.Framework;
using Transition.Compiler.ValueConverters;

namespace Tests.Compiler
{
   [TestFixture]
   public class ValueConverterTests
   {
      [Test]
      public void FloatConverter_ValueIsProperFloat_ParsesFloat()
      {
         var input = "1.205";

         object result;
         var parseSuccessful = new FloatValueConverter().TryConvert(input, out result);

         Assert.IsTrue(parseSuccessful);
         Assert.AreEqual(1.205f, (float)result);
      }

      [Test]
      public void FloatConverter_ValueIsNotFloat_ReturnsFalse()
      {
         var input = "bad_input";

         object result;
         var parseSuccessful = new FloatValueConverter().TryConvert(input, out result);

         Assert.IsFalse(parseSuccessful);
      }

      [Test]
      public void IntConverter_ValueIsPropeInt_ParsesInt()
      {
         var input = "123";

         object result;
         var parseSuccessful = new IntValueConverter().TryConvert(input, out result);

         Assert.IsTrue(parseSuccessful);
         Assert.AreEqual(123, (int)result);
      }

      [Test]
      public void IntConverter_ValueIsNotInt_ReturnsFalse()
      {
         var input = "bad_int_input";

         object result;
         var parseSuccessful = new IntValueConverter().TryConvert(input, out result);

         Assert.IsFalse(parseSuccessful);
      }

      [Test]
      public void StringConverter_AnyInput_ReturnsString()
      {
         var input = "1.205";

         object result;
         var parseSuccessful = new StringValueConverter().TryConvert(input, out result);

         Assert.IsTrue(parseSuccessful);
         Assert.IsInstanceOf<string>(result);
      }
   }
}

