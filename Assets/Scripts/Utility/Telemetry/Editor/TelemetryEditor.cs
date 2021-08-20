using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TelemetryDebugger))]
public class TelemetryEditor : Editor
{
    private TelemetryDebugger m_Target;
    private bool m_DrawData = false;
    private List<Vector3> m_ActivePositions = new List<Vector3>();

    private void OnEnable()
    {
        m_Target = target as TelemetryDebugger;
    }

    public override void OnInspectorGUI()
    {
        if (m_Target == null) return;

        m_Target.MaxTracesPerTile = (ushort)EditorGUILayout.IntField("Max Overlap", m_Target.MaxTracesPerTile);
        m_Target.Threshold = EditorGUILayout.Slider("Data Threshold", m_Target.Threshold, 0.0f, 1.0f);
        m_Target.TraceColourCurve = EditorGUILayout.GradientField("Colour Curve", m_Target.TraceColourCurve);

        if (GUILayout.Button("Generate Data"))
        {
            m_Target.ClearTracers();

            Dictionary<string, List<Vector3>> telemetryData = Telemetry.Retrieve();

            GameObject containerObject = new GameObject();

            GameObject tracerObject = new GameObject("Tracker");
            tracerObject.AddComponent<PositionTracer>();
            m_ActivePositions = new List<Vector3>();

            foreach (var dataset in telemetryData)
            {
                var tracerContainer = Instantiate(containerObject, m_Target.transform.position, Quaternion.identity, m_Target.transform);
                tracerContainer.name = dataset.Key;

                foreach (var position in dataset.Value)
                {
                    if (m_ActivePositions.Contains(position))
                    {
                        foreach (var tracer in m_Target.PositionObjects)
                        {
                            if (tracer.transform.position == position)
                            {
                                tracer.Traces++;
                                break;
                            }
                        }
                    }
                    else
                    {
                        var tracerComponent = Instantiate(tracerObject, position, Quaternion.identity, tracerContainer.transform).GetComponent<PositionTracer>();
                        m_ActivePositions.Add(position);
                        m_Target.PositionObjects.Add(tracerComponent);
                    }
                }
            }

            DestroyImmediate(containerObject);
            DestroyImmediate(tracerObject);
        }

        if (GUILayout.Button("Clear Data") && m_Target != null)
            m_Target.ClearTracers();

        m_DrawData = GUILayout.Toggle(m_DrawData, "Draw Data");
        m_Target.DrawData(m_DrawData);
    }
}