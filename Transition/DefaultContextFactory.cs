namespace Transition
{
   /// <summary>
   /// An IContextFactory that supplies the base Context class. Used with the DefaultStateMachineController
   /// </summary>
   public class DefaultContextFactory : IContextFactory<Context>
   {
      public Context BuildContext()
      {
         return new Context();
      }
   }
}
