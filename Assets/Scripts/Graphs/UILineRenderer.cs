using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UILineRenderer : Graphic
{
    public List<float> values = new List<float>();
    public float lineWidth = 2.0f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (values.Count < 2)
            return;   

        // Plot the last 100 values
        int start = 0;
        if (values.Count > 100)
            start = values.Count - 100;

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        float max = Mathf.Max(values.ToArray()) + 10;
        float min = Mathf.Min(values.ToArray()) - 10;

        float xSize = width / 100.0f;
        float ySize = height / (max - min);

        List<Vector2> points = new List<Vector2>();

        for (int i = start; i < values.Count; i++)
        {
            float x = (i - start) * xSize - width / 2;
            float y = (values[i] - min) * ySize - height / 2;

            points.Add(new Vector2(x, y));
        }

        float halfLineWidth = lineWidth / 2;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 pointA = points[i];
            Vector2 pointB = points[i + 1];
            Vector2 direction = (pointB - pointA).normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x) * halfLineWidth;

            Vector2 v0 = pointA - normal;
            Vector2 v1 = pointA + normal;
            Vector2 v2 = pointB + normal;
            Vector2 v3 = pointB - normal;

            vh.AddVert(v0, color, Vector2.zero);
            vh.AddVert(v1, color, Vector2.zero);
            vh.AddVert(v2, color, Vector2.zero);
            vh.AddVert(v3, color, Vector2.zero);

            int index = vh.currentVertCount - 4;
            vh.AddTriangle(index, index + 1, index + 2);
            vh.AddTriangle(index + 2, index + 3, index);
        }
    }

    public void AddValue(float value)
    {
        values.Add(value);
        SetVerticesDirty();
    }
}
