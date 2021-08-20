using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TelemetryDebugger : MonoBehaviour
{
    // Yuck...
    public static TelemetryDebugger Instance { get; private set; }

    public ushort MaxTracesPerTile { get; set; } = 50;
    public float Threshold { get; set; } = 0.0f;
    public Gradient TraceColourCurve { get; set; } = new Gradient();
    public List<PositionTracer> PositionObjects { get; set; } = new List<PositionTracer>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogWarning("Telemetry debugger already exists in scene.");
            Destroy(gameObject);
        }
    }

    public void DrawData(bool draw)
    {
        foreach (var tracer in PositionObjects)
        {
            if (tracer != null)
            {
                if (draw && (float)tracer.Traces / MaxTracesPerTile < Threshold)
                    tracer.gameObject.SetActive(false);
                else
                    tracer.gameObject.SetActive(draw);
            }
        }

        PositionObjects.RemoveAll(tracer => tracer == null);
    }

    public void ClearTracers()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        PositionObjects.Clear();
    }
}
