using UnityEngine;

public class Raccoon : Entity
{
    private enum RaccoonState { MovingToCenter, Crossing, MovingToExit, Completed }
    private RaccoonState currentState = RaccoonState.MovingToCenter;

    private Vector3 startPosition;
    private Vector3 exitPosition;
    private Vector3 centerPosition;
    private float journeyTime;
    private float phaseDuration = 3f;

    protected override bool UsePolarMovement => false;

    protected override void InitializeMovement()
    {
        entityColor = new Color(0.4f, 0.4f, 0.4f);

        // Стартовая позиция
        startPosition = transform.position;

        // Противоположная точка выхода
        float startAngle = Mathf.Atan2(startPosition.y, startPosition.x);
        float exitAngle = (startAngle + Mathf.PI) % (2f * Mathf.PI);
        exitPosition = CoordinateConverter.PolarToWorld2D(new Vector2(Entity.SPAWN_RADIUS, exitAngle));

        // Центр
        centerPosition = Vector3.zero;

        // Скорость (можно переопределить в инспекторе)
        EnsureMoveSpeed(60f);

        journeyTime = 0f;
        currentState = RaccoonState.MovingToCenter;

        Debug.Log($"Енот: старт {startPosition}, выход {exitPosition}");
    }

    protected override void Move()
    {
        if (currentState == RaccoonState.Completed)
            return;

        journeyTime += Time.deltaTime;
        float progress = journeyTime / phaseDuration;

        switch (currentState)
        {
            case RaccoonState.MovingToCenter:
                MoveToCenter(progress);
                break;

            case RaccoonState.Crossing:
                CrossCenter(progress);
                break;

            case RaccoonState.MovingToExit:
                MoveToExit(progress);
                break;
        }
    }

    private void MoveToCenter(float progress)
    {
        transform.position = Vector3.Lerp(startPosition, centerPosition, progress);

        if (progress >= 1f)
        {
            currentState = RaccoonState.Crossing;
            journeyTime = 0f;
            Debug.Log("Енот достиг центра");
        }
    }

    private void CrossCenter(float progress)
    {
        // Вращаемся в центре
        float radius = 15f;
        float angle = progress * 2f * Mathf.PI;

        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius,
            0
        );

        transform.position = centerPosition + offset;
        transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

        if (progress >= 1f)
        {
            currentState = RaccoonState.MovingToExit;
            journeyTime = 0f;
            Debug.Log("Енот закончил осмотр центра");
        }
    }

    private void MoveToExit(float progress)
    {
        transform.position = Vector3.Lerp(centerPosition, exitPosition, progress);
        transform.rotation = Quaternion.identity;

        if (progress >= 1f)
        {
            currentState = RaccoonState.Completed;
            Debug.Log("Енот достиг выхода и исчезает");
            DestroyEntity();
        }
    }
}
