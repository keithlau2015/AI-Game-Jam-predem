using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class Blackboard
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public void Set<T>(string key, T value)
        {
            _data[key] = value;
        }

        public bool TryGet<T>(string key, out T value)
        {
            if (_data.TryGetValue(key, out object obj) && obj is T typed)
            {
                value = typed;
                return true;
            }
            value = default;
            return false;
        }

        public T GetOrDefault<T>(string key, T defaultValue = default)
        {
            return TryGet(key, out T v) ? v : defaultValue;
        }

        public bool Has(string key)
        {
            return _data.ContainsKey(key);
        }

        public void Remove(string key)
        {
            _data.Remove(key);
        }

        public void Clear()
        {
            _data.Clear();
        }
    }
}
