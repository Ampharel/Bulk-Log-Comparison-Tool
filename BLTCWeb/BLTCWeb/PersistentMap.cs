using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

    public abstract class PersistentMap<T, U>
    {
        private readonly string _filePath;
        protected readonly Dictionary<T, U> _map;

        protected PersistentMap(string filePath)
        {
            _filePath = filePath;
            _map = Load();
        }

        public void Set(T key, U value)
        {
            _map[key] = value;
            if (AutoSave)
            {
                Save();
            }
        }

        public bool Remove(T key)
        {
            var removed = _map.Remove(key);
            if (AutoSave)
            {
                Save();
            }
            return removed;
        }

        public U? Get(T key)
        {
            return _map.TryGetValue(key, out var value) ? value : default(U);
        }

        public bool Contains(T key)
        {
            return _map.ContainsKey(key);
        }
        public IEnumerable<KeyValuePair<T, U>> GetAll()
        {
            return _map;
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(_map);
            File.WriteAllText(_filePath, json);
        }

        public void Clear()
        {
            _map.Clear();
        }

        protected virtual bool AutoSave => false;

        private Dictionary<T, U> Load()
        {
            if (!File.Exists(_filePath))
                return new();

            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<Dictionary<T, U>>(json) ?? new();
            }
            catch
            {
                return new();
            }
        }
    }
