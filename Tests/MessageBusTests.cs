using Transition;
using NUnit.Framework;

namespace Tests
{
   [TestFixture]
   public class MessageBusTests
   {
      [Test]
      public void EnqueueMessage_MessageWithoutValue_MessageEnqueued()
      {
         var messageBus = new MessageBus(4);

         messageBus.EnqueueMessage("a", 2, null);
         var envelope = messageBus.Dequeue();

         Assert.AreEqual("a", envelope.Key);
         Assert.AreEqual(2, envelope.RecipientContextId);
         Assert.IsNull(envelope.Value);
      }

      [Test]
      public void EnqueueMessage_MessageWithValue_EnqueuedMessageContainsValue()
      {
         var messageBus = new MessageBus(4);

         messageBus.EnqueueMessage("a", 2, "asdf");
         var envelope = messageBus.Dequeue();

         Assert.AreEqual("a", envelope.Key);
         Assert.AreEqual(2, envelope.RecipientContextId);
         Assert.AreEqual("asdf", envelope.Value);
      }

      [Test]
      [ExpectedException(typeof(System.Exception))]
      public void EnqueueMessage_NotEnoughCapacity_ExceptionIsThrown()
      {
         var messageBus = new MessageBus(1);

         messageBus.EnqueueMessage("a", 2, "asdf");
         // second should throw
         messageBus.EnqueueMessage("a", 2, "asdf");
      }

      [Test]
      public void Dequeue_StartsWithOneMessage_CountBecomesZero()
      {
         var messageBus = new MessageBus(4);
         messageBus.EnqueueMessage("a", 2, "asdf");

         messageBus.Dequeue();

         Assert.AreEqual(0, messageBus.Count);
      }

      [Test]
      public void Dequeue_StartsWithOneMessage_NextDequeueReturnsNull()
      {
         var messageBus = new MessageBus(4);
         messageBus.EnqueueMessage("a", 2, "asdf");

         messageBus.Dequeue();
         var secondEnvelope = messageBus.Dequeue();

         Assert.IsNull(secondEnvelope);
      }

      [Test]
      public void Recycle_MessageHasAllValuesSet_MessageValuesReturnedToDefaultState()
      {
         var messageBus = new MessageBus(4);
         messageBus.EnqueueMessage("a", 2, "asdf");

         var messageEnvelope = messageBus.Dequeue();
         messageBus.Recycle(messageEnvelope);

         Assert.IsNull(messageEnvelope.Key);
         Assert.IsNull(messageEnvelope.Value);
         Assert.AreEqual(-1, messageEnvelope.RecipientContextId);
      }
   }
}
