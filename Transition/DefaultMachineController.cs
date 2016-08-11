namespace Transition
{
   /// <summary>
   /// A simple StateMachineController that uses the base Context.
   /// </summary>
   public class DefaultMachineController : MachineController<Context>
   {
      protected override Context BuildContext()
      {
         return new Context();
      }
   }
}
