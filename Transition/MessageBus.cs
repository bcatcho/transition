using System;

namespace Transition
{
   public class MessageBus
   {
      /// <summary>
      /// Envelopes are stored in a pre-instantiated ring buffer for garbage-collection-less operations.
      /// </summary>
      private MessageEnvelope[] _envelopes;
      private int _head;

      /// <summary>
      /// The numver of messages in the bus
      /// </summary>
      public int Count { get; private set; }

      private int _capacity;

      public MessageBus(int capacity)
      {
         _capacity = capacity;
         _head = 0;
         Count = 0;
         _envelopes = new MessageEnvelope[_capacity];
         // instantiate a pool of message envelopes
         for (int i = 0; i < _capacity; ++i) {
            _envelopes[i] = new MessageEnvelope();
         }
      }

      /// <summary>
      /// EnqueueMessage creates a message envelope and stores it for future, in-order processing
      /// </summary>
      /// <param name="messageKey">The key of the message that appears in the "on" section of a State.</param>
      /// <param name="recipientContextId">The contextId of the receiver of the message.</param>
      /// <param name="messageValue">Message value if any.</param>
      public void EnqueueMessage(string messageKey, int recipientContextId, object messageValue)
      {
         if (Count >= _capacity) {
            throw new Exception("MessageBus ring buffer ran out of space, initialize with a larger capacity.");
         }
         var nextPosition = (_head + Count) % _capacity;
         var envelope = _envelopes[nextPosition];
         envelope.RecipientContextId = recipientContextId;
         envelope.Key = messageKey;
         envelope.Value = messageValue;
         Count++;
      }

      /// <summary>
      /// Get the next available message in the bus
      /// </summary>
      /// <returns>The first message or null.</returns>
      public MessageEnvelope Dequeue()
      {
         if (Count == 0) {
            return null;
         }
         var result = _envelopes[_head];
         // wrap around
         _head = (_head + 1) % _capacity;
         Count -= 1;
         return result;
      }

      /// <summary>
      /// Recycle an envelope when after it has been sent. This helps to avoid garbage collection
      /// </summary>
      public void Recycle(MessageEnvelope envelope)
      {
         envelope.Key = null;
         envelope.RecipientContextId = -1;
         envelope.Value = null;
      }
   }
}
