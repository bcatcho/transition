using System.Collections.Generic;

namespace Transition
{
   /// <summary>
   /// A Blackboard is a container for shared state that is local to a context. This allows actions to share 
   /// state and communicate with each other. It is meant to help make actions more reusable.
   /// </summary>
   public class Blackboard
   {
      private readonly Dictionary<string, object> _dataMap;

      public Blackboard()
      {
         _dataMap = new Dictionary<string, object>();
      }

      public T Get<T>(string name)
      {
         return (T)_dataMap[name];
      }

      public void Set<T>(string name, T val)
      {
         _dataMap[name] = val;
      }

      public bool Exists(string name)
      {
         return _dataMap.ContainsKey(name);
      }
   }
}
