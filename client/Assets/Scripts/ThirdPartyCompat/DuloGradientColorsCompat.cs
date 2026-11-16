using UnityEngine;
using UnityEngine.UI;

// Replacement for the optional Asset Store UI gradient component.
public sealed class DuloGradientColorsCompat : BaseMeshEffect
{
    public Color topColor = Color.white;
    public Color bottomColor = Color.white;

    public override void ModifyMesh(VertexHelper vertexHelper)
    {
        if (!IsActive())
        {
            return;
        }

        var vertices = new System.Collections.Generic.List<UIVertex>();
        vertexHelper.GetUIVertexStream(vertices);
        if (vertices.Count == 0)
        {
            return;
        }

        var minY = float.PositiveInfinity;
        var maxY = float.NegativeInfinity;
        foreach (var vertex in vertices)
        {
            minY = Mathf.Min(minY, vertex.position.y);
            maxY = Mathf.Max(maxY, vertex.position.y);
        }

        var height = Mathf.Max(0.0001f, maxY - minY);
        for (var index = 0; index < vertices.Count; index++)
        {
            var vertex = vertices[index];
            var t = Mathf.InverseLerp(minY, maxY, vertex.position.y);
            vertex.color = Color32.Lerp(bottomColor, topColor, t);
            vertices[index] = vertex;
        }

        vertexHelper.Clear();
        vertexHelper.AddUIVertexTriangleStream(vertices);
    }
}
