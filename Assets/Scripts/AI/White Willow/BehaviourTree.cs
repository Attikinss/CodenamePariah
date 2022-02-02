using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WhiteWillow.Nodes;

namespace WhiteWillow
{
    [CreateAssetMenu(fileName = "New Behaviour Tree", menuName = "White Willow/Behaviour Tree")]
    public class BehaviourTree : ScriptableObject
    {
        public Root RootNode;
        public Agent Agent { get; private set; }
        public Blackboard Blackboard;

        public List<BaseNode> Nodes = new List<BaseNode>();

        public void SetAgent(Agent agent) => Agent = agent;
        public void SetBlackboard(Blackboard blackboard) => Blackboard = blackboard;

        public BehaviourTree Clone(string name)
        {
            // Shallow copy the tree
            BehaviourTree newTree = ScriptableObject.Instantiate(this);
            newTree.name = $"[{name}] Behaviour Tree";

            // Clear the nodes
            newTree.Nodes.Clear();

            // Add shallow copies of original nodes
            Nodes.ForEach(node => newTree.Nodes.Add(ScriptableObject.Instantiate(node)));

            newTree.Nodes.ForEach(node =>
            {
                node.Owner = newTree;

                var root = node as Root;
                if (root != null)
                {
                    var childNode = newTree.Nodes.Find(itr => itr.GUID == root.Child.GUID /*string.CompareOrdinal(itr.GUID, root.Child.GUID) == 0*/);
                    root.ClearChild();
                    root.SetChild(childNode);
                    newTree.RootNode = root;
                }

                var composite = node as Composite;
                if (composite != null)
                {
                    var children = newTree.Nodes.Where(itr => composite.Children.Any(child => itr.GUID == child.GUID
                        /*string.CompareOrdinal(itr.GUID, child.GUID) == 0*/)).ToArray();
                    composite.Children.Clear();
                    composite.SetRunningChild(null);
                    composite.AddChildren(children);
                }

                var decorator = node as Decorator;
                if (decorator != null)
                {
                    var childNode = newTree.Nodes.Find(itr => itr.GUID == decorator.Child.GUID /*string.CompareOrdinal(itr.GUID, decorator.Child.GUID) == 0*/);
                    decorator.ClearChild();
                    decorator.SetChild(childNode);
                }
            });

            return newTree;
        }

        public void Tick() => RootNode?.Tick();

        public BaseNode CreateRoot()
        {
            var rootNode = Nodes.Find(node => node.GetType() == typeof(Root)) as Root;
            if (rootNode != null)
            {
                RootNode = rootNode;
                return rootNode;
            }

            var node = CreateInstance(typeof(Root)) as Root;
            node.Owner = this;
            node.name = node.GetType().Name;
            node.Title = node.name;
            node.GUID = Guid.NewGuid().ToString();
            Nodes.Add(node);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.AddObjectToAsset(node, this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
            RootNode = node;
            return node;
        }

        public BaseNode CreateNode(Type type)
        {
            var node = CreateInstance(type) as BaseNode;
            node.Owner = this;
            node.name = type.Name;
            node.Title = node.name;
            node.GUID = Guid.NewGuid().ToString();
            Nodes.Add(node);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.AddObjectToAsset(node, this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif

            return node;
        }

        public BaseNode CreateNode(BaseNode node)
        {
            var newNode = CreateInstance(node.GetType()) as BaseNode;
            newNode.Owner = this;
            newNode.Title = !string.IsNullOrWhiteSpace(node.Title) ? node.Title : newNode.GetType().Name;
            newNode.name = newNode.GetType().Name;
            newNode.GUID = Guid.NewGuid().ToString();
            Nodes.Add(newNode);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.AddObjectToAsset(newNode, this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif

            return newNode;
        }

        public void DeleteNode(BaseNode node)
        {
            Nodes.Remove(node);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.RemoveObjectFromAsset(node);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }
}