using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class CrashCurveGraphic : MaskableGraphic
{
    private readonly List<Vector2> points = new List<Vector2>();

    [Header("Line Settings")]
    public float lineWidth = 7f;
    public Color lineColor = new Color(0.0f, 0.95f, 0.65f, 1f);
    public Color lineEndColor = new Color(0.0f, 0.9f, 1f, 1f);
    public Color crashLineColor = new Color(0.95f, 0.25f, 0.25f, 1f);

    [Header("Fill Settings")]
    public bool drawFill = true;
    public Color fillTopColor = new Color(0.0f, 0.95f, 0.7f, 0.28f);
    public Color fillBottomColor = new Color(0.02f, 0.12f, 0.2f, 0.0f);
    public Color crashFillTopColor = new Color(0.95f, 0.25f, 0.25f, 0.16f);
    public float baselineY = 0f;

    [Header("State")]
    public bool isCrashed = false;

    public void UpdateCurve(List<Vector2> newPoints, float baseLine, bool crashed)
    {
        points.Clear();
        if (newPoints != null && newPoints.Count > 0)
        {
            points.AddRange(newPoints);
        }
        baselineY = baseLine;
        isCrashed = crashed;
        SetVerticesDirty();
    }

    public void ClearCurve()
    {
        points.Clear();
        isCrashed = false;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        int count = points.Count;
        if (count < 2) return;

        Color curLineStart = isCrashed ? crashLineColor : lineColor;
        Color curLineEnd = isCrashed ? crashLineColor : lineEndColor;
        Color curFillTop = isCrashed ? crashFillTopColor : fillTopColor;
        Color curFillBottom = isCrashed ? new Color(0.2f, 0.04f, 0.04f, 0f) : fillBottomColor;

        if (drawFill)
        {
            for (int i = 0; i < count - 1; i++)
            {
                Vector2 p0 = points[i];
                Vector2 p1 = points[i + 1];

                float t0 = (float)i / (count - 1);
                float t1 = (float)(i + 1) / (count - 1);

                Color top0 = Color.Lerp(curFillTop, curLineEnd * new Color(1, 1, 1, curFillTop.a), t0);
                Color top1 = Color.Lerp(curFillTop, curLineEnd * new Color(1, 1, 1, curFillTop.a), t1);

                int vIdx = vh.currentVertCount;
                vh.AddVert(new Vector3(p0.x, baselineY, 0), curFillBottom, Vector2.zero);
                vh.AddVert(new Vector3(p0.x, p0.y, 0), top0, Vector2.zero);
                vh.AddVert(new Vector3(p1.x, p1.y, 0), top1, Vector2.zero);
                vh.AddVert(new Vector3(p1.x, baselineY, 0), curFillBottom, Vector2.zero);

                vh.AddTriangle(vIdx, vIdx + 1, vIdx + 2);
                vh.AddTriangle(vIdx, vIdx + 2, vIdx + 3);
            }
        }

        float halfW = lineWidth * 0.5f;
        Vector2[] normals = new Vector2[count];

        for (int i = 0; i < count; i++)
        {
            Vector2 dir;
            if (i == 0)
            {
                dir = (points[1] - points[0]).normalized;
            }
            else if (i == count - 1)
            {
                dir = (points[count - 1] - points[count - 2]).normalized;
            }
            else
            {
                Vector2 d1 = (points[i] - points[i - 1]).normalized;
                Vector2 d2 = (points[i + 1] - points[i]).normalized;
                dir = ((d1 + d2) * 0.5f).normalized;
                if (dir.sqrMagnitude < 0.001f) dir = d1;
            }
            normals[i] = new Vector2(-dir.y, dir.x);
        }

        int lineStartVert = vh.currentVertCount;
        for (int i = 0; i < count; i++)
        {
            Vector2 p = points[i];
            Vector2 n = normals[i];
            float t = (float)i / (count - 1);
            Color c = Color.Lerp(curLineStart, curLineEnd, t);

            vh.AddVert(new Vector3(p.x - n.x * halfW, p.y - n.y * halfW, 0), c, Vector2.zero);
            vh.AddVert(new Vector3(p.x + n.x * halfW, p.y + n.y * halfW, 0), c, Vector2.zero);
        }

        for (int i = 0; i < count - 1; i++)
        {
            int baseIdx = lineStartVert + i * 2;
            vh.AddTriangle(baseIdx, baseIdx + 1, baseIdx + 3);
            vh.AddTriangle(baseIdx, baseIdx + 3, baseIdx + 2);
        }
    }
}
