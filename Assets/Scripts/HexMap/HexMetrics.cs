using UnityEngine;

public static class HexMetrics
{

    public const float outerRadius = 1f;

    public const float innerRadius = outerRadius * 0.866025404f;//something something pythagorean theorem

    public static Vector3[] corners = {
        new Vector3(0f, outerRadius, 0f),
        new Vector3(innerRadius, 0.5f * outerRadius, 0f),
        new Vector3(innerRadius, -0.5f * outerRadius, 0f),
        new Vector3(0f, -outerRadius, 0f),
        new Vector3(-innerRadius, -0.5f * outerRadius, 0f),
        new Vector3(-innerRadius, 0.5f * outerRadius, 0f),
        new Vector3(0f, outerRadius, 0f)

    };
}
