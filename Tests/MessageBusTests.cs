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
         var envelope = messageBus.DequeueFirst();

         Assert.AreEqual("a", envelope.Key);
         Assert.AreEqual(2, envelope.RecipientContextId);
         Assert.IsNull(envelope.Value);
      }
   }
}
