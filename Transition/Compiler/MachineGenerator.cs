using Transition.Compiler.AstNode;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace Transition.Compiler
{
   /// <summary>
   /// The "code" generator for the compiler. Accepts a validated syntax tree and returns an executable Machine.
   /// </summary>
   public class MachineGenerator<T> where T : Context
   {
      private Dictionary<string, Type> _actionLookupTable;
      //      private Dictionary<System.Type, IBehaviorTreeCompilerValueConverter> _valueConverterLookup;
      private HashSet<Assembly> _loadedAssemblies;
      //      private Dictionary<string, Dictionary<string, PropertyInfo>> _propertyInfoCache;
      //      private Dictionary<string, PropertyInfo> _defaultPropertyInfoCache;

      public MachineGenerator()
      {
         _actionLookupTable = new Dictionary<string, Type>();
//         _valueConverterLookup = new Dictionary<System.Type, IBehaviorTreeCompilerValueConverter>();
         _loadedAssemblies = new HashSet<Assembly>();
//         _propertyInfoCache = new Dictionary<string, Dictionary<string, PropertyInfo>>();
//         _defaultPropertyInfoCache = new Dictionary<string, PropertyInfo>();
      }

      /// <summary>
      /// Call before generate to build a lookup table of Actions to instantiate during generation. This method
      /// uses reflection to look for decendants of "Action" and cache anything that is used to instantiate them.
      /// </summary>
      public void Initialize(params Assembly[] assemblies)
      {
         // default assembly
         LoadAssembly(Assembly.GetAssembly(typeof(MachineGenerator<>)));

         if (assemblies != null) {
            foreach (var assembly in assemblies) {
               LoadAssembly(assembly);
            }
         }
      }

      /// <summary>
      /// Generates an executable Machine from a syntax tree
      /// </summary>
      /// <param name="machineAst">Machine syntax tree node</param>
      public Machine<T> Generate(MachineAstNode machineAst)
      {
         var machine = new Machine<T>
         {
            Identifier = machineAst.Identifier,
         };
         machine.EnterAction = GenerateAction(machineAst.Action);

         for (int i = 0; i < machineAst.States.Count; ++i) {
            machine.AddState(GenerateState(machineAst.States[i]));
         }

         return machine;
      }

      private State<T> GenerateState(StateAstNode stateNode)
      {
         var state = new State<T>
         {
            Identifier = stateNode.Identifier,
         };
         if (stateNode.Run != null) {
            for (int i = 0; i < stateNode.Run.Actions.Count; ++i) {
               state.AddRunAction(GenerateAction(stateNode.Run.Actions[i]));
            }
         }
         if (stateNode.Enter != null) {
            for (int i = 0; i < stateNode.Enter.Actions.Count; ++i) {
               state.AddEnterAction(GenerateAction(stateNode.Enter.Actions[i]));
            }
         }
         if (stateNode.Exit != null) {
            for (int i = 0; i < stateNode.Exit.Actions.Count; ++i) {
               state.AddExitAction(GenerateAction(stateNode.Exit.Actions[i]));
            }
         }
         if (stateNode.On != null) {
            for (int i = 0; i < stateNode.On.Actions.Count; ++i) {
               state.AddOnAction(stateNode.On.Actions[i].Message, GenerateAction(stateNode.On.Actions[i]));
            }
         }
         return state;
      }

      private Action<T> GenerateAction(ActionAstNode actionNode)
      {
         return CreateInstance(actionNode.Identifier);
      }

      private Action<T> CreateInstance(string actionIdentifier)
      {
         if (!_actionLookupTable.ContainsKey(actionIdentifier)) {
            throw new KeyNotFoundException(string.Format("Could not find action for name [{0}]", actionIdentifier));
         }
         var type = _actionLookupTable[actionIdentifier];
         if (type.IsGenericType) {
            type = type.MakeGenericType(typeof(T));
         }

         return (Action<T>)Activator.CreateInstance(type);
      }

      private void LoadAssembly(Assembly assembly)
      {
         if (!_loadedAssemblies.Contains(assembly)) {
            _loadedAssemblies.Add(assembly);
            LoadValueConverters(assembly);
            LoadAllTasks(assembly);
         }
      }

      private void LoadValueConverters(Assembly assembly)
      {
//         var converterType = typeof(IBehaviorTreeCompilerValueConverter);
//         var types = assembly.GetTypes();
//         IBehaviorTreeCompilerValueConverter converter;
//         foreach (var type in types) {
//            if (converterType.IsAssignableFrom(type) && !type.IsInterface) {
//               converter = (IBehaviorTreeCompilerValueConverter)System.Activator.CreateInstance(type);
//               _valueConverterLookup.Add(converter.GetConverterType(), converter);
//            }
//         }
      }

      private void LoadAllTasks(Assembly assembly)
      {
         var action = typeof(Transition.Action<>);
         var types = assembly.GetTypes();
         Type genericDef = null;
         string name = null;
         foreach (var type in types) {
            if ((type.BaseType != null && type.BaseType.IsGenericType)) {
               genericDef = type.BaseType.GetGenericTypeDefinition();
               if (genericDef == action) {
                  // lowercase the names and remove generic param
                  name = type.Name.Split('`')[0];
                  AddLookupTableId(name, type);

                  if (type.IsDefined(typeof(AltIdAttribute), true)) {
                     var altIdAttribs = (AltIdAttribute[])type.GetCustomAttributes(typeof(AltIdAttribute), true);
                     foreach (var attrib in altIdAttribs) {
                        foreach (var altId in attrib.AltIds) {
                           AddLookupTableId(altId, type);
                        }
                     }
                  }
               }
            }
         }
      }

      private void AddLookupTableId(string name, Type type)
      {
         name = name.ToLower();
         _actionLookupTable.Add(name, type);
      }
   }
}
