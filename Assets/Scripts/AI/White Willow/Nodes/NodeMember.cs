using System;
using UnityEngine;

namespace WhiteWillow
{
    [Serializable]
    public class NodeMember<T>
    {
        public bool BlackboardValue = false;

        [HideInInspector]
        public T Value;

        [HideInInspector]
        public bool Expand = false;

        [HideInInspector]
        public int Selection = 0;
    }
}
