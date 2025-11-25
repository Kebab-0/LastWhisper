using System.Collections;
using UnityEngine;

public class CommonFreak : Entity
{
    private enum FreakState { Wandering, CheckingThreats, AttackingBunker, Dead }
    private FreakState currentState = FreakState.Wandering;

    private float wanderTime = 8f;
    private float wanderTimer;
    private float spawnAngle;
    private Vector2 wanderCenter;

    protected override void InitializeMovement()
    {
        entityColor = new Color(0.6f, 0.0f, 0.0f);

        spawnAngle = Random.Range(0f, 2f * Mathf.PI);
        polarPosition = new Vector2(800f, spawnAngle);
        wanderTimer = wanderTime;

        // Центр блуждания - случайная точка недалеко от точки появления
        wanderCenter = new Vector2(600f, spawnAngle);

        moveSpeed = 90f;

        Debug.Log($"Урод обыкновенный создан. Начальный угол: {spawnAngle * Mathf.Rad2Deg:F1}°");
    }

    protected override void Move()
    {
        if (currentState == FreakState.Dead) return;

        switch (currentState)
        {
            case FreakState.Wandering:
                Wander();
                break;

            case FreakState.AttackingBunker:
                AttackBunker();
                break;
        }
    }

    private void Wander()
    {
        wanderTimer -= Time.deltaTime;

        // Блуждание по небольшой области
        float newRadius = wanderCenter.x + Mathf.Sin(Time.time * 0.5f) * 100f;
        float newAngle = wanderCenter.y + Mathf.Sin(Time.time * 0.3f) * 0.5f;

        polarPosition = new Vector2(newRadius, newAngle);

        if (wanderTimer <= 0)
        {
            currentState = FreakState.AttackingBunker;
            Debug.Log("Урод начинает атаку на бункер");
        }

        // Отладочная информация
        if (Mathf.FloorToInt(Time.time) % 5 == 0 && Mathf.FloorToInt(Time.time) != Mathf.FloorToInt(Time.time - Time.deltaTime))
        {
            Debug.Log($"Урод блуждает: радиус {polarPosition.x:F0}, угол {polarPosition.y * Mathf.Rad2Deg:F1}°");
        }
    }

    private void AttackBunker()
    {
        Vector2 bunkerPosition = new Vector2(0f, 0f);
        polarPosition = Vector2.MoveTowards(polarPosition, bunkerPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(polarPosition, bunkerPosition) < 100f)
        {
            StartCoroutine(BunkerAttackRoutine());
        }
    }

    private IEnumerator BunkerAttackRoutine()
    {
        Debug.Log("Урод атакует бункер");
        yield return new WaitForSeconds(2f);
        TakeDamage(1000f);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        if (currentHealth <= 0)
        {
            currentState = FreakState.Dead;
            Debug.Log("Урод убит электричеством бункера");
        }
    }
}