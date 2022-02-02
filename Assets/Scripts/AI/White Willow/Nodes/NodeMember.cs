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
        public string Name = "";

        [HideInInspector]
        public int Selection = 0;

        public void Validate(Blackboard blackboard)
        {
            if (BlackboardValue)
            {
                var value = (T)blackboard.GetEntry<T>(Name)?.Value;

                if (value != null)
                    Value = value;
            }
        }
    }
}
