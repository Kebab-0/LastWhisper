using UnityEngine;
using System.Collections;

public class Repressor : Entity
{
    private enum RepressorState { Moving, Attacking, Leaving }
    private RepressorState currentState = RepressorState.Moving;

    private SectorManager sectorManager;
    private float targetAngle;
    private float attackRadius = 300f;

    protected override void InitializeMovement()
    {
        entityColor = new Color(0.5f, 0.0f, 0.5f);

        sectorManager = SectorManager.Instance;

        Scanner lastScanner = sectorManager?.GetLastDestroyedScanner();
        if (lastScanner != null)
        {
            targetAngle = lastScanner.PolarPosition.y;
        }
        else
        {
            targetAngle = Random.Range(0f, 2f * Mathf.PI);
        }

        polarPosition = new Vector2(800f, targetAngle);
        currentState = RepressorState.Moving;

        moveSpeed = 150f;

        Debug.Log($"Репрессор создан. Целевой угол: {targetAngle * Mathf.Rad2Deg:F1}°");
    }

    protected override void Move()
    {
        switch (currentState)
        {
            case RepressorState.Moving:
                MoveToSector();
                break;

            case RepressorState.Attacking:
                Attack();
                break;

            case RepressorState.Leaving:
                Leave();
                break;
        }
    }

    private void MoveToSector()
    {
        Vector2 targetPosition = new Vector2(attackRadius, targetAngle);
        polarPosition = Vector2.MoveTowards(polarPosition, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(polarPosition, targetPosition) < 10f)
        {
            currentState = RepressorState.Attacking;
            Debug.Log("Репрессор начал атаку");
        }
    }

    private void Attack()
    {
        StartCoroutine(AttackRoutine());
        currentState = RepressorState.Leaving;
    }

    private IEnumerator AttackRoutine()
    {
        Debug.Log("Репрессор атакует врагов в секторе");
        yield return new WaitForSeconds(2f);
        Debug.Log("Репрессор завершил атаку");
    }

    private void Leave()
    {
        Vector2 exitPosition = new Vector2(800f, targetAngle);
        polarPosition = Vector2.MoveTowards(polarPosition, exitPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(polarPosition, exitPosition) < 10f)
        {
            Debug.Log("Репрессор покинул диск");
            DestroyEntity();
        }
    }
}