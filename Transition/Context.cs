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
      public ErrorCode LastError { get; set; }

      /// <summary>
      /// The current index of the currently running action in a State's RunActions section.
      /// </summary>
      public int ActionIndex { get; set; }

      /// <summary>
      /// The Identifier of the currently active state.
      /// </summary>
      public int StateId { get; set; }

      /// <summary>
      /// The identifier for the Machine that uses this context.
      /// </summary>
      public string MachineIdentifier { get; set; }

      /// <summary>
      /// The Message property will be set when a state is responding to a message. This allows
      /// actions to inspect the details or payload of the message.
      /// </summary>
      public MessageEnvelope Message { get; set; }

      public Context()
      {
         // this allows the machine to know if it has run for the very first time
         StateId = -1;
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
