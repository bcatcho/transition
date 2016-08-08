using Transition.Compiler.AstNode;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace Transition.Compiler
{
   /// <summary>
   /// The "code" generator for the compiler. Accepts a validated syntax tree and returns an executable Machine.
   /// </summary>
   public class MachineGenerator
   {
      private Dictionary<string, System.Type> _taskLookupTable;
      //      private Dictionary<System.Type, IBehaviorTreeCompilerValueConverter> _valueConverterLookup;
      private HashSet<Assembly> _loadedAssemblies;
      private Dictionary<string, Dictionary<string, PropertyInfo>> _propertyInfoCache;
      private Dictionary<string, PropertyInfo> _defaultPropertyInfoCache;

      /// <summary>
      /// Call before generate to build a lookup table of Actions to instantiate during generation. This method
      /// uses reflection to look for decendants of "Action" and cache anything that is used to instantiate them.
      /// </summary>
      public void Initialize(params Assembly[] assemblies)
      {
         // default assembly
         LoadAssembly(Assembly.GetAssembly(typeof(MachineGenerator)));

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
      public Machine Generate(MachineAstNode machineAst)
      {
         var machine = new Machine
         {
            Identifier = machineAst.Identifier,
         };

         return machine;
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
         var action = typeof(Action);
         var types = assembly.GetTypes();
         Type genericDef = null;
         string name = null;
         foreach (var type in types) {
            if ((type.BaseType != null && type.BaseType.IsGenericType)) {
               genericDef = type.BaseType.GetGenericTypeDefinition();
               if (genericDef == action) {
                  // lowercase the names and remove generic param
                  name = type.Name.Split('`')[0];
                  AddLookupTableId(name, type, genericDef);

//                  if (type.IsDefined(typeof(AltIdAttribute), true)) {
//                     var altIdAttribs = (AltIdAttribute[])type.GetCustomAttributes(typeof(AltIdAttribute), true);
//                     foreach (var attrib in altIdAttribs) {
//                        foreach (var altId in attrib.AltIds) {
//                           AddLookupTableId(altId, type, genericDef);
//                        }
//                     }
//                  }
               }
            }
         }
      }

      private void AddLookupTableId(string name, Type type, Type genericTypeDef)
      {
         name = name.ToLower();
         _taskLookupTable.Add(name, type);
      }
   }
}
