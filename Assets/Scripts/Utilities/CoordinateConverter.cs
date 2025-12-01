using UnityEngine;

public static class CoordinateConverter
{
    public static Vector3 PolarToWorld2D(Vector2 polar)
    {
        float radius = polar.x;
        float angle = polar.y;

        return new Vector3(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius,
            0
        );
    }

    public static Vector2 WorldToPolar2D(Vector3 world)
    {
        float radius = world.magnitude;
        float angle = Mathf.Atan2(world.y, world.x);

        return new Vector2(radius, angle);
    }

    // Для обратной совместимости
    public static Vector3 PolarToWorld(Vector2 polar) => PolarToWorld2D(polar);
    public static Vector2 WorldToPolar(Vector3 world) => WorldToPolar2D(world);
}