namespace Transition
{
   /// <summary>
   /// A simple StateMachineController that uses the base Context.
   /// </summary>
   public class DefaultMachineController<T1> : MachineController<T1, Context>
   {
      protected override Context BuildContext()
      {
         return new Context();
      }
   }
}
