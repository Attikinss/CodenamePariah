using System.Collections;
using UnityEngine;

public class PositionTracer : MonoBehaviour
{
    [ReadOnly]
    public ushort Traces = 1;

    private void OnDrawGizmos()
    {
        ushort traceQuantity = (ushort)Mathf.Clamp(Traces, 0, TelemetryDebugger.Instance.MaxTracesPerTile);

        Gizmos.color = TelemetryDebugger.Instance.TraceColourCurve.Evaluate((float)traceQuantity / TelemetryDebugger.Instance.MaxTracesPerTile);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}