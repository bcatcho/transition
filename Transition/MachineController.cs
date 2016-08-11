using System.Collections.Generic;
using Transition.Compiler;

namespace Transition
{
   /// <summary>
   /// A MachineController is used to instantiate and run all State Machines. It is provided as a convenient way
   /// to get started with Transition but is not necessary. At the very least it serves as a template for the entire
   /// Machine lifecycle from compilation to execution.
   /// 
   /// It has two generic parameters.
   /// T1 represents the way in which you wish to associate a Context with a Machine. For instance, say you 
   /// are making a game where each state-machine-driven agent is assigned an integer Id. In this case you would
   /// instantiate a "StateMachineController<int, Context>". Then later, when you wish to run the state machine for
   /// a specific agent, you would write "stageMachineController.Tick(agent.Id)". The controller will locate the 
   /// correct context by the integer Id, match it with the Machine that it is for and will call "tick" on the machine.
   /// 
   /// T2 is the Context that will besupplied to all actions. This is a way of injecting your own custom context
   /// into each action as it is processed. The DefaultStateMachineController uses the base Context class.
   /// </summary>
   public abstract class MachineController<T1, T2> where T2 : Context
   {
      private readonly Dictionary<string, Machine<T2>> _machineMap;
      private readonly Dictionary<T1, T2> _contextMap;
      private MachineCompiler<T2> _compiler;

      protected MachineController()
      {
         _contextMap = new Dictionary<T1, T2>();
         _compiler = new MachineCompiler<T2>();
         _machineMap = new Dictionary<string, Machine<T2>>();
      }

      /// <summary>
      /// This is a factory function for Contexts. Use this to supply your own custom Context for each machine instance.
      /// </summary>
      protected abstract T2 BuildContext();

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
         _machineMap.Add(machine.Identifier, machine);
      }

      /// <summary>
      /// Creates and returns a new Context and associates it with the id. Call this before ticking the 
      /// Machine with that Id. 
      /// The Context is returned for further customization by the caller. 
      /// </summary>
      public T2 AddMachineInstance(T1 id, string machineIdentifier)
      {
         if (!_machineMap.ContainsKey(machineIdentifier)) {
            throw new KeyNotFoundException("A Machine not found for name " + machineIdentifier);
         }
         var context = BuildContext();
         context.MachineId = machineIdentifier;
         _contextMap.Add(id, context);

         // return the context for further customization
         return context;
      }

      /// <summary>
      /// Runs the Machine for the given id
      /// </summary>
      public void Tick(T1 id)
      {
         var context = _contextMap[id];
         var machine = _machineMap[context.MachineId];
         machine.Tick(context);
      }
   }
}
