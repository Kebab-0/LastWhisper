using UnityEngine;

public class Deer : Entity
{
    private enum DeerState { MovingToArea, Wandering, Returning }
    private DeerState currentState = DeerState.MovingToArea;

    private float stateTimer = 0f;
    private Vector3 wanderCenter;
    private Vector3 currentTarget;
    private Vector3 spawnPosition;
    private float wanderRadius = 30f;
    private float areaReachedDistance = 5f;
    private float maxWanderTime = 8f;
    private float returnChance = 0.3f;

    protected override bool UsePolarMovement => false;

    protected override void InitializeMovement()
    {
        entityColor = Color.yellow;

        // Спавн позиция
        spawnPosition = transform.position;

        // Центр блуждания внутри рабочей зоны
        float centerDistance = Random.Range(50f, Entity.WORK_RADIUS * 0.8f);
        float centerAngle = Random.Range(0f, 2f * Mathf.PI);
        wanderCenter = CoordinateConverter.PolarToWorld2D(new Vector2(centerDistance, centerAngle));

        // Первая цель
        currentTarget = GetRandomWanderPoint();

        // Скорость (можно переопределить в инспекторе)
        EnsureMoveSpeed(50f);

        stateTimer = 0f;
        currentState = DeerState.MovingToArea;

        Debug.Log($"Олень создан. Центр блуждания: {wanderCenter}");
    }

    protected override void Move()
    {
        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case DeerState.MovingToArea:
                MoveToWanderArea();
                break;

            case DeerState.Wandering:
                Wander();
                break;

            case DeerState.Returning:
                ReturnToSpawn();
                break;
        }
    }

    private void MoveToWanderArea()
    {
        MoveTowardsWorld(wanderCenter);

        if (Vector3.Distance(transform.position, wanderCenter) < areaReachedDistance || stateTimer > 10f)
        {
            currentState = DeerState.Wandering;
            stateTimer = 0f;
            currentTarget = GetRandomWanderPoint();
            Debug.Log("Олень достиг области блуждания");
        }
    }

    private void Wander()
    {
        MoveTowardsWorld(currentTarget);

        bool targetReached = Vector3.Distance(transform.position, currentTarget) < areaReachedDistance;
        bool timeExpired = stateTimer > maxWanderTime;

        if (targetReached || timeExpired)
        {
            if (Random.Range(0f, 1f) < returnChance)
            {
                currentState = DeerState.Returning;
                stateTimer = 0f;
                Debug.Log("Олень начинает возвращение к месту спавна");
            }
            else
            {
                currentTarget = GetRandomWanderPoint();
                stateTimer = 0f;
            }
        }
    }

    private void ReturnToSpawn()
    {
        MoveTowardsWorld(spawnPosition);

        if (Vector3.Distance(transform.position, spawnPosition) < areaReachedDistance || stateTimer > 15f)
        {
            Debug.Log("Олень вернулся к точке спавна и исчезает");
            DestroyEntity();
        }
    }

    private Vector3 GetRandomWanderPoint()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        Vector3 point = wanderCenter + new Vector3(randomOffset.x, randomOffset.y, 0);

        // Не выходим за рабочий радиус
        if (point.magnitude > Entity.WORK_RADIUS)
        {
            point = point.normalized * Entity.WORK_RADIUS * 0.9f;
        }

        return point;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // При приближении волка - убегаем быстрее
        MutantWolf wolf = other.GetComponent<MutantWolf>();
        if (wolf != null)
        {
            moveSpeed *= 1.5f; // Ускоряемся
            currentState = DeerState.Returning; // Начинаем возврат
            stateTimer = 0f;
            Debug.Log("Олень испугался волка!");
        }
    }
}
