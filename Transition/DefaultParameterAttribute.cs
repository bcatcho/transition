using System;

namespace Transition
{
   /// <summary>
   /// Use this attribute to simplify an action statement that takes a single value.
   /// 
   /// Given the class:
   /// 
   /// public class Wait : Action<Context> {
   ///   [DefaultParameter]
   ///   public int Seconds { get; set; }
   /// }
   /// 
   /// Either of these state machine actions are valid:
   /// 
   /// @state Blah
   ///   @run
   ///     wait seconds:"3"
   ///     wait "3"
   /// </summary>
   [AttributeUsage(AttributeTargets.Property)]
   public class DefaultParameterAttribute : Attribute
   {
   }
}
