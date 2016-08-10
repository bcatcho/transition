namespace Transition
{
   /// <summary>
   /// Implement this interface to build your own custom Context which will be passed into a Machine when run.
   /// </summary>
   public interface IContextFactory<T> where T : Context
   {
      T BuildContext();
   }
}
