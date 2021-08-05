using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace WhiteWillow.Editor
{
    public class EdgeView : Edge
    {
        public EdgeView() : base()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/Edge"));
        }

        public void Highlight()
        {
        }

        public void Unhighlight()
        {
        }
    }
}
