namespace Transition
{
   /// <summary>
   /// A simple StateMachineController that uses the base Context.
   /// </summary>
   public class DefaultMachineController : MachineController<Context>
   {
      public DefaultMachineController() : base(500, 5000)
      {
      }

      protected override Context BuildContext()
      {
         return new Context();
      }
   }
}
