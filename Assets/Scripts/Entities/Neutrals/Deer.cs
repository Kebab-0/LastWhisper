using UnityEngine;

public class Deer : Entity
{
    private enum DeerState { MovingToArea, Wandering, Returning }
    private DeerState currentState = DeerState.MovingToArea;

    private float stateTimer = 0f;
    private Vector2 spawnPolarPosition;
    private Vector2 wanderCenter;
    private Vector2 currentTarget;
    private float wanderRadius = 300f;
    private float minRadius = 100f;

    protected override void InitializeMovement()
    {
        entityColor = Color.yellow;

        // Появление на границе
        float spawnAngle = Random.Range(0f, 2f * Mathf.PI);
        spawnPolarPosition = new Vector2(800f, spawnAngle);
        polarPosition = spawnPolarPosition;

        // Центр блуждания - случайная точка внутри диска
        float centerRadius = Random.Range(200f, 600f);
        float centerAngle = Random.Range(0f, 2f * Mathf.PI);
        wanderCenter = new Vector2(centerRadius, centerAngle);

        // Первая целевая точка для блуждания
        currentTarget = GetRandomWanderPoint();

        moveSpeed = 60f;

        Debug.Log($"Олень создан. Точка появления: радиус {spawnPolarPosition.x:F0}, угол {spawnAngle * Mathf.Rad2Deg:F1}°");
        Debug.Log($"Центр блуждания: радиус {wanderCenter.x:F0}, угол {wanderCenter.y * Mathf.Rad2Deg:F1}°");
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
        // Движение к области блуждания
        polarPosition = Vector2.MoveTowards(polarPosition, wanderCenter, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(polarPosition, wanderCenter) < 50f || stateTimer > 8f)
        {
            currentState = DeerState.Wandering;
            stateTimer = 0f;
            currentTarget = GetRandomWanderPoint();
            Debug.Log("Олень достиг области блуждания");
        }
    }

    private void Wander()
    {
        // Движение к текущей целевой точке
        polarPosition = Vector2.MoveTowards(polarPosition, currentTarget, moveSpeed * 0.5f * Time.deltaTime);

        // Если достигли цели или прошло много времени, выбираем новую цель
        if (Vector2.Distance(polarPosition, currentTarget) < 20f || stateTimer > 6f)
        {
            currentTarget = GetRandomWanderPoint();
            stateTimer = 0f;
        }

        // Проверяем, не пора ли возвращаться
        if (stateTimer > 15f)
        {
            currentState = DeerState.Returning;
            stateTimer = 0f;
            Debug.Log("Олень начинает возвращаться к точке появления");
        }

        // Отладочная информация
        if (Mathf.FloorToInt(Time.time) % 8 == 0 && Mathf.FloorToInt(Time.time) != Mathf.FloorToInt(Time.time - Time.deltaTime))
        {
            Debug.Log($"Олень блуждает: радиус {polarPosition.x:F0}, угол {polarPosition.y * Mathf.Rad2Deg:F1}°");
        }
    }

    private void ReturnToSpawn()
    {
        // Прямолинейное возвращение к точке появления
        polarPosition = Vector2.MoveTowards(polarPosition, spawnPolarPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(polarPosition, spawnPolarPosition) < 10f)
        {
            Debug.Log("Олень вернулся к точке появления и исчезает");
            DestroyEntity();
        }

        // Отладочная информация
        if (Mathf.FloorToInt(Time.time) % 3 == 0 && Mathf.FloorToInt(Time.time) != Mathf.FloorToInt(Time.time - Time.deltaTime))
        {
            Debug.Log($"Олень возвращается: радиус {polarPosition.x:F0}, угол {polarPosition.y * Mathf.Rad2Deg:F1}°");
        }
    }

    private Vector2 GetRandomWanderPoint()
    {
        // Случайная точка в области блуждания
        float randomRadius = wanderCenter.x + Random.Range(-wanderRadius, wanderRadius);
        randomRadius = Mathf.Clamp(randomRadius, minRadius, 750f);

        float randomAngle = wanderCenter.y + Random.Range(-Mathf.PI / 2, Mathf.PI / 2);

        return new Vector2(randomRadius, randomAngle);
    }
}