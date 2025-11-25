using UnityEngine;

public static class CoordinateConverter
{
    public static Vector3 PolarToWorld2D(Vector2 polarCoord)
    {
        float radius = polarCoord.x;
        float angle = polarCoord.y;
        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);
        return new Vector3(x, y, 0);
    }

    public static Vector2 WorldToPolar2D(Vector3 worldPos)
    {
        float radius = Mathf.Sqrt(worldPos.x * worldPos.x + worldPos.y * worldPos.y);
        float angle = Mathf.Atan2(worldPos.y, worldPos.x);
        return new Vector2(radius, angle);
    }

    public static Vector3 PolarToWorld(Vector2 polarCoord)
    {
        return PolarToWorld2D(polarCoord);
    }

    public static Vector2 WorldToPolar(Vector3 worldPos)
    {
        return WorldToPolar2D(worldPos);
    }

    public static Vector3 PolarToWorld3D(Vector2 polarCoord, float yHeight = 0f)
    {
        float radius = polarCoord.x;
        float angle = polarCoord.y;
        float x = radius * Mathf.Cos(angle);
        float z = radius * Mathf.Sin(angle);
        return new Vector3(x, yHeight, z);
    }
}