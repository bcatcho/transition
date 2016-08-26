using System;
using System.Collections.Generic;

namespace Transition
{
   /// <summary>
   /// Use this attribute to apply a different name for an action. Supply an array of strings to use as
   /// alternative names in the your state machine definition. This is useful when you want to add a shoter
   /// name for frequently used actions while allowing the class name to be very descriptive.
   /// 
   /// Given the class:
   /// 
   /// [AltId("wait")]
   /// public class WaitForManySeconds : Action<Context> { }
   /// 
   /// Either of these state machine actions are valid:
   /// 
   /// @state Blah
   ///   @run
   ///     wait
   ///     waitForManySeconds
   /// </summary>
   [AttributeUsage(AttributeTargets.Class)]
   public class AltIdAttribute : Attribute
   {
      public List<string> AltIds { get; set; }

      public AltIdAttribute(params string[] altIds)
      {
         AltIds = new List<string>(altIds);
      }
   }
}
