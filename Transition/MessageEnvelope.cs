namespace Transition
{
   /// <summary>
   /// A container for messages sent to state machine actions
   /// </summary>
   public class MessageEnvelope
   {
      /// <summary>
      /// The context id of the recipient
      /// </summary>
      public int RecipientContextId { get; set; }
      /// <summary>
      /// The identifier of the message
      /// </summary>
      public string Key { get; set; }

      /// <summary>
      /// The payload (if any) of the message
      /// </summary>
      public object Value { get; set; }
   }
}
