using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteWillow
{
    [CreateAssetMenu(fileName = "New Blackboard", menuName = "White Willow/Blackboard")]
    public class Blackboard : ScriptableObject
    {
        public List<BlackboardEntry> Entries = new List<BlackboardEntry>();
        public bool Updated { get; set; } = false;

        public void AddEntry<T>(string name, T value)
        {
            if (Entries.Exists(e => e.Name == name /*string.CompareOrdinal(e.Name, name) == 0*/))
            {
                Debug.LogWarning($"An entry with the name <{name}> is already on the blackboard!");
                return;
            }

            BlackboardEntry entry = null;
            string assetName = "";

            if (typeof(T) == typeof(bool))
            {
                assetName = "Bool Entry";
                entry = ScriptableObject.CreateInstance<BoolEntry>();
            }
            else if (typeof(T) == typeof(float))
            {
                assetName = "Float Entry";
                entry = ScriptableObject.CreateInstance<FloatEntry>();
            }
            else if (typeof(T) == typeof(GameObject))
            {
                assetName = "GameObject Entry";
                entry = ScriptableObject.CreateInstance<GameObjectEntry>();
            }
            else if (typeof(T) == typeof(int))
            {
                assetName = "Int Entry";
                entry = ScriptableObject.CreateInstance<IntEntry>();
            }
            else if (typeof(T) == typeof(string))
            {
                assetName = "String Entry";
                entry = ScriptableObject.CreateInstance<StringEntry>();
            }
            else if (typeof(T) == typeof(Vector3))
            {
                assetName = "Vector Entry";
                entry = ScriptableObject.CreateInstance<VectorEntry>();
            }
            else if (typeof(T) == typeof(object))
            {
                assetName = "Empty Entry";
                entry = ScriptableObject.CreateInstance<EmptyEntry>();
            }
            else
            {
                Debug.LogWarning($"Entry types of <{typeof(T).Name}> are not supported!");
                return;
            }
            
            entry.Name = name;
            entry.name = assetName;
            entry.Value = value;

#if UNITY_EDITOR

            Entries.Add(entry);

            UnityEditor.AssetDatabase.AddObjectToAsset(entry, this);
            UnityEditor.AssetDatabase.SaveAssets();

#endif

            Updated = true;
        }

        public BlackboardEntry ReplaceEntry(BlackboardEntry a, BlackboardEntry b)
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.RemoveObjectFromAsset(a);
            UnityEditor.AssetDatabase.AddObjectToAsset(b, this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif

            int index = Entries.FindIndex(entry => entry == a);
            Entries[index] = b;

            Updated = true;

            return Entries[index];
        }

        public void RemoveEntry(string name)
        {
            var entry = Entries.FirstOrDefault(e => e.Name == name /*string.CompareOrdinal(e.Name, name) == 0*/);

            if (entry == null)
            {
                Debug.LogWarning($"No entry with the name <{name}> exists on the blackboard!");
                return;
            }

#if UNITY_EDITOR

            Entries.Remove(entry);

            UnityEditor.AssetDatabase.RemoveObjectFromAsset(entry);
            UnityEditor.AssetDatabase.SaveAssets();
            ScriptableObject.DestroyImmediate(entry);

#endif

            Updated = true;
        }

        public BlackboardEntry GetEntry<T>(string name)
        {
            var entry = Entries.FirstOrDefault(e => e.Name == name /*string.CompareOrdinal(e.Name, name) == 0*/);

            if (entry != null)
                return entry;

            Debug.LogWarning($"No entry with the name <{name}> exists on the blackboard!");
            return null;
        }

        public BlackboardEntry this[string name]
        {
            get => Entries.FirstOrDefault(e => e.Name == name /*string.CompareOrdinal(e.Name, name) == 0*/);
        }

        public object UpdateEntryValue<T>(string name, object value)
        {
            var entry = Entries.FirstOrDefault(e => e.Name == name /*string.CompareOrdinal(e.Name, name) == 0*/);

            if (!entry.Equals(null))
            {
                entry.Value = (T)value;
                Updated = true;

                return entry.Value;
            }

            Debug.LogWarning($"No entry with the name <{name}> exists on the blackboard!");
            return null;
        }

        public void UpdateEntryName(string name, string newName)
        {
            if (name == newName) return;

            var entry = Entries.FirstOrDefault(e => e.Name == name /*string.CompareOrdinal(e.Name, name) == 0*/);

            if (!entry.Equals(null))
            {
                entry.name = newName;
                Updated = true;

                return;
            }

            Debug.LogWarning($"No entry with the name <{name}> exists on the blackboard!");
        }

        public List<BlackboardEntry> GetEntriesOfType(ValueTypes type)
        {
            List<BlackboardEntry> entries = new List<BlackboardEntry>();

            foreach (var entry in Entries)
            {
                if (entry.ValueType == type)
                    entries.Add(entry);
            }

            return entries;
        }

        public List<BlackboardEntry> GetEntriesOfType(Type type)
        {
            List<BlackboardEntry> entries = new List<BlackboardEntry>();
        
            foreach (var entry in Entries)
            {
                ValueTypes value = entry.ValueType;

                if ((type == typeof(bool) && value == ValueTypes.Bool) ||
                    (type == typeof(float) && value == ValueTypes.Float) ||
                    (type == typeof(GameObject) && value == ValueTypes.GameObject) ||
                    (type == typeof(int) && value == ValueTypes.Int) ||
                    (type == typeof(string) && value == ValueTypes.String) ||
                    (type == typeof(Vector3) && value == ValueTypes.Vector))
                    entries.Add(entry);
            }
        
            return entries;
        }
    }
}