using System.Collections.Generic;
using Transition.Compiler;
using System;

namespace Transition
{
   /// <summary>
   /// A MachineController is used to instantiate and run all State Machines. It is provided as a convenient way
   /// to get started with Transition but is not necessary. At the very least it serves as a template for the entire
   /// Machine lifecycle from compilation to execution.
   /// 
   /// T is the Context that will besupplied to all actions. This is a way of injecting your own custom context
   /// into each action as it is processed. The DefaultStateMachineController uses the base Context class.
   /// </summary>
   public abstract class MachineController<T> where T : Context
   {
      /// <summary>
      /// The machineIdMap should only be used during Context creation as dictionary lookups are slow.
      /// </summary>
      private readonly Dictionary<string, int> _machineIdMap;
      private readonly Dictionary<int, int> _contextIdToArrayPosMap;
      private readonly Machine<T>[] _machines;
      private int _machineCount;
      private readonly T[] _contexts;
      private int _contextCount;
      private int _nextContextId;
      private MachineCompiler<T> _compiler;
      protected MessageBus MessageBus;

      protected MachineController(int messageBusCapacity)
      {
         _contextIdToArrayPosMap = new Dictionary<int, int>(5000);
         _machines = new Machine<T>[200];
         _contexts = new T[5000];
         _compiler = new MachineCompiler<T>();
         _machineIdMap = new Dictionary<string, int>();
         MessageBus = new MessageBus(messageBusCapacity);
      }

      /// <summary>
      /// This is a factory function for Contexts. Use this to supply your own custom Context for each machine instance.
      /// </summary>
      protected abstract T BuildContext();

      /// <summary>
      /// Loads Actions that will be used in future compiled machines
      /// </summary>
      public void LoadActions(params Type[] actions)
      {
         _compiler.LoadActions(actions);
      }

      /// <summary>
      /// Loads ValueConverters that will be used future compiled machines
      /// </summary>
      public void LoadValueConverters(params Type[] valueConverters)
      {
         _compiler.LoadValueConverters(valueConverters);
      }

      /// <summary>
      /// Compiles a string into an executable Machine and stores that machine by it's Identifier.
      /// Use this Identifier to assign the machine to a context.
      /// </summary>
      public void Compile(string input)
      {
         var machine = _compiler.Compile(input);
         _machines[_machineCount] = machine;
         _machineIdMap.Add(machine.Identifier, _machineCount);
         _machineCount++;
      }

      /// <summary>
      /// Creates and returns a new Context and associates it with the id. Call this before ticking the 
      /// Machine with that Id. 
      /// The Context is returned for further customization by the caller. 
      /// </summary>
      public T AddMachineInstance(string machineIdentifier)
      {
         if (!_machineIdMap.ContainsKey(machineIdentifier)) {
            throw new KeyNotFoundException("A Machine not found for name " + machineIdentifier);
         }
         var context = BuildContext();
         context.MessageBus = MessageBus;
         context.MachineId = _machineIdMap[machineIdentifier];
         context.ContextId = _nextContextId;
         _contextIdToArrayPosMap.Add(context.ContextId, _contextCount);
         _contexts[_contextCount] = context;
         _contextCount++;
         _nextContextId++;

         // return the context for further customization
         return context;
      }

      /// <summary>
      /// Remove a context id from the list. Will no longer be ticked
      /// </summary>
      public void RemoveMachineInstance(int contextId)
      {
         var arrayPos = _contextIdToArrayPosMap[contextId];
         var contextToRemove = _contexts[arrayPos];
         contextToRemove.Reset();
         _contextIdToArrayPosMap.Remove(contextId);

         // now compact the list of contexts by copying the last into the array pos
         _contexts[arrayPos] = _contexts[_contextCount - 1];
         _contextIdToArrayPosMap[_contexts[arrayPos].ContextId] = arrayPos;
         // set the last index to null and reduce the count
         _contexts[_contextCount - 1] = null;
         _contextCount--;
      }

      /// <summary>
      /// Runs the Machine for the given the contextId
      /// NOTE: This is very slow due to dictionary lookup! If you have many machines to run use
      /// TickAll instead
      /// </summary>
      public void Tick(int contextId)
      {
         var arrayPos = _contextIdToArrayPosMap[contextId];
         var context = _contexts[arrayPos];
         var machine = _machines[context.MachineId];
         machine.Tick(context);
      }

      /// <summary>
      /// Delivers all messages in the MessageBus
      /// </summary>
      public void DeliverQueuedMessages()
      {
         T context;
         Machine<T> machine;
         MessageEnvelope envelope;
         while (MessageBus.Count > 0) {
            envelope = MessageBus.Dequeue();
            context = _contexts[envelope.RecipientContextId];
            machine = _machines[context.MachineId];
            machine.SendMessage(context, envelope);
            MessageBus.Recycle(envelope);
         }
      }

      /// <summary>
      /// Runs the Machine for all contexts
      /// </summary>
      public void TickAll()
      {
         // deliver any messages sent by external components since last tick
         DeliverQueuedMessages();
         // tick all machines
         T context;
         Machine<T> machine;
         for (int i = 0; i < _contextCount; ++i) {
            context = _contexts[i];
            machine = _machines[context.MachineId];
            machine.Tick(context);
         }
         // deliver any messages enqueued by Machines during this tick
         DeliverQueuedMessages();
      }

      /// <summary>
      /// Send a message to a Machine
      /// </summary>
      /// <param name="messageKey">Message key (as per the "@on" section of a State.</param>
      /// <param name="recipientContextId">Recipient's context identifier.</param>
      /// <param name="value">Optional Value</param>
      public void SendMessage(string messageKey, int recipientContextId, object value)
      {
         MessageBus.EnqueueMessage(messageKey, recipientContextId, value);
      }
   }
}
