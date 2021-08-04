using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GraphicalDebugger : MonoBehaviour
{
    [Tooltip("Make sure you drag in a gameobject that's within the scene.")]
    public Canvas m_debugCanvasPrefab;

    /// <summary>
    /// Assigns variable to be displayed for a text element.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="varToDisplay">Variable you want to display.</param>
    /// <param name="prefixText">Text to display before the variable.</param>">
    /// <param name="textObj">The text element.</param>
    public static void Assign<T>(T varToDisplay, string prefixText, Text textObj)
    {
        textObj.text = prefixText + ": " + varToDisplay.ToString();
    }


    /// <summary>
    /// Draws sphere casts.
    /// </summary>
    /// <param name="start">Start point.</param>
    /// <param name="end">End point.</param>
    public static void DrawSphereCast(Vector3 start, Vector3 end, Color colour, float radius)
    {
        Color defaultColour = Gizmos.color;
        Gizmos.color = colour;

        

        for (int i = 0; i < 5; i++)
        {
            Vector3 circlePoint = Vector3.Lerp(start, end, (float)i / 5);
            Gizmos.DrawWireSphere(circlePoint, radius);
        }

        Gizmos.color = defaultColour;
    }
}

