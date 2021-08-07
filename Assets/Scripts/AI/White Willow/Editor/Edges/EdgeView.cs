using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace WhiteWillow.Editor
{
    public class EdgeView : Edge
    {
        public EdgeView() : base()
        {
            edgeControl.inputColor = Color.white;
            edgeControl.outputColor = Color.white;
            edgeControl.fromCapColor = Color.white;
            edgeControl.toCapColor = Color.white;
        }

        public void Highlight()
        {
            edgeControl.inputColor = Color.green;
            edgeControl.outputColor = Color.green;
            edgeControl.fromCapColor = Color.green;
            edgeControl.toCapColor = Color.green;
        }

        public void Unhighlight()
        {
            edgeControl.inputColor = Color.white;
            edgeControl.outputColor = Color.white;
            edgeControl.fromCapColor = Color.white;
            edgeControl.toCapColor = Color.white;
        }
    }
}
