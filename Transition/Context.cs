namespace Transition
{
   /// <summary>
   /// The context is a datum that represents the current state of a single Machine. 
   /// A Machine will only ever be instantiated once but may be used by many different actors
   /// in a simulation. Each actor would have it's own context. When a Machine is Tick'ed it
   /// is given the context specific to each actor. This reduces memory needed to store the 
   /// Machine tree and makes serialization simpler.
   /// </summary>
   public class Context
   {
      /// <summary>
      /// Gets the last error raised (if any). This should be reset before each tick.
      /// </summary>
      public ErrorCode LastError { get; private set; }

      /// <summary>
      /// The execution state for the associated machine.
      /// </summary>
      public ExecutionState ExecState { get; private set; }

      public Context()
      {
         ExecState = new ExecutionState();
      }

      /// <summary>
      /// Resets the LastError code to None
      /// </summary>
      public void ResetError()
      {
         LastError = ErrorCode.None;
      }

      /// <summary>
      /// Sets LastError.
      /// Used to emit error codes in Actions, States or the Machine itself.
      /// </summary>
      public void RaiseError(ErrorCode code)
      {
         LastError = code;
      }
   }
}
