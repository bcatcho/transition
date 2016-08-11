using System.Collections.Generic;
using Transition.Compiler;

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
      private readonly Machine<T>[] _machines;
      private int _machineCount;
      private readonly T[] _contexts;
      private int _contextCount;
      private MachineCompiler<T> _compiler;

      protected MachineController()
      {
         _machines = new Machine<T>[200];
         _contexts = new T[5000];
         _compiler = new MachineCompiler<T>();
         _machineIdMap = new Dictionary<string, int>();
      }

      /// <summary>
      /// This is a factory function for Contexts. Use this to supply your own custom Context for each machine instance.
      /// </summary>
      protected abstract T BuildContext();

      /// <summary>
      /// Call this method to initialize the compiler. Only needs to be run once and before any compilation.
      /// </summary>
      public void InitializeCompiler(params System.Reflection.Assembly[] assemblies)
      {
         _compiler.Initialize(assemblies);
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
         context.MachineId = _machineIdMap[machineIdentifier];
         context.ContextId = _contextCount;
         _contexts[_contextCount] = context;
         _contextCount++;

         // return the context for further customization
         return context;
      }

      /// <summary>
      /// Runs the Machine for the given the contextId
      /// </summary>
      public void Tick(int contextId)
      {
         var context = _contexts[contextId];
         var machine = _machines[context.MachineId];
         machine.Tick(context);
      }

      /// <summary>
      /// Runs the Machine for all contexts
      /// </summary>
      public void TickAll()
      {
         T context;
         Machine<T> machine;
         for (int i = 0; i < _contextCount; ++i) {
            context = _contexts[i];
            machine = _machines[context.MachineId];
            machine.Tick(context);
         }
      }
   }
}
