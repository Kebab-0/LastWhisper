using System.Collections;
using UnityEngine;

public class MutantWolf : Entity
{
    private enum WolfState
    {
        MovingToFirstSector,
        SearchingFirstSector,
        MovingToSecondSector,
        SearchingSecondSector,
        ChasingNeutral,
        Killing,
        MovingToBunker,
        AttackingBunker,
        Dead
    }

    private WolfState currentState = WolfState.MovingToFirstSector;
    private SectorManager sectorManager;
    private Entity currentTarget;
    private Vector3 firstSectorPoint;
    private Vector3 secondSectorPoint;
    private Vector3 spawnPosition;
    private float killRange = 10f;
    private float searchRadius = 100f;
    private float stateTimer = 0f;

    // Используем мировые координаты
    protected override bool UsePolarMovement => false;

    protected override void InitializeMovement()
    {
        entityColor = Color.red;

        // Получаем спавн позицию
        spawnPosition = transform.position;

        // Получаем SectorManager
        sectorManager = SectorManager.Instance;
        if (sectorManager == null)
        {
            Debug.LogError("SectorManager не найден!");
            return;
        }

        // Первый сектор - ближе к месту спавна
        Vector2 firstSectorPolar = CoordinateConverter.WorldToPolar2D(spawnPosition);
        firstSectorPolar.x = Mathf.Min(firstSectorPolar.x * 0.7f, Entity.WORK_RADIUS * 0.7f);
        firstSectorPoint = CoordinateConverter.PolarToWorld2D(firstSectorPolar);

        // Второй сектор - противоположная сторона
        Vector2 secondSectorPolar = firstSectorPolar;
        secondSectorPolar.y = (secondSectorPolar.y + Mathf.PI) % (2f * Mathf.PI);
        secondSectorPolar.x = Entity.WORK_RADIUS * 0.4f;
        secondSectorPoint = CoordinateConverter.PolarToWorld2D(secondSectorPolar);

        // Скорость (можно переопределить в инспекторе)
        EnsureMoveSpeed(80f);

        currentState = WolfState.MovingToFirstSector;
        stateTimer = 0f;

        Debug.Log($"Волк создан. Спавн: {spawnPosition}, Первый сектор: {firstSectorPoint}");
    }

    protected override void Move()
    {
        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case WolfState.MovingToFirstSector:
                MoveToFirstSector();
                break;

            case WolfState.SearchingFirstSector:
                SearchFirstSector();
                break;

            case WolfState.MovingToSecondSector:
                MoveToSecondSector();
                break;

            case WolfState.SearchingSecondSector:
                SearchSecondSector();
                break;

            case WolfState.ChasingNeutral:
                ChaseNeutral();
                break;

            case WolfState.MovingToBunker:
                MoveToBunker();
                break;

            case WolfState.AttackingBunker:
                AttackBunker();
                break;

            case WolfState.Killing:
                // Ждем завершения убийства
                break;

            case WolfState.Dead:
                // Уже мертв
                break;
        }
    }

    private void MoveToFirstSector()
    {
        MoveTowardsWorld(firstSectorPoint);

        if (Vector3.Distance(transform.position, firstSectorPoint) < 5f || stateTimer > 15f)
        {
            currentState = WolfState.SearchingFirstSector;
            stateTimer = 0f;
            Debug.Log("Волк достиг первого сектора");
        }
    }

    private void SearchFirstSector()
    {
        // Ищем нейтралов в секторе
        float angle = Mathf.Atan2(firstSectorPoint.y, firstSectorPoint.x);
        var neutrals = sectorManager.GetNeutralsInSector(angle, searchRadius);

        if (neutrals.Count > 0)
        {
            currentTarget = neutrals[0];
            currentState = WolfState.ChasingNeutral;
            Debug.Log($"Волк обнаружил цель: {currentTarget.name}");
        }
        else if (stateTimer > 5f)
        {
            currentState = WolfState.MovingToSecondSector;
            stateTimer = 0f;
            Debug.Log("В первом секторе нет целей, двигаюсь ко второму");
        }
    }

    private void MoveToSecondSector()
    {
        MoveTowardsWorld(secondSectorPoint);

        if (Vector3.Distance(transform.position, secondSectorPoint) < 5f || stateTimer > 15f)
        {
            currentState = WolfState.SearchingSecondSector;
            stateTimer = 0f;
            Debug.Log("Волк достиг второго сектора");
        }
    }

    private void SearchSecondSector()
    {
        // Ищем нейтралов в секторе
        float angle = Mathf.Atan2(secondSectorPoint.y, secondSectorPoint.x);
        var neutrals = sectorManager.GetNeutralsInSector(angle, searchRadius);

        if (neutrals.Count > 0)
        {
            currentTarget = neutrals[0];
            currentState = WolfState.ChasingNeutral;
            Debug.Log($"Волк обнаружил цель во втором секторе: {currentTarget.name}");
        }
        else if (stateTimer > 5f)
        {
            currentState = WolfState.MovingToBunker;
            stateTimer = 0f;
            Debug.Log("Целей нет, двигаюсь к бункеру");
        }
    }

    private void ChaseNeutral()
    {
        if (currentTarget == null)
        {
            currentState = WolfState.MovingToSecondSector;
            return;
        }

        MoveTowardsWorld(currentTarget.transform.position);

        if (Vector3.Distance(transform.position, currentTarget.transform.position) < killRange)
        {
            StartCoroutine(KillNeutral());
        }
    }

    private IEnumerator KillNeutral()
    {
        currentState = WolfState.Killing;

        if (currentTarget != null)
        {
            Debug.Log($"Волк убивает {currentTarget.name}");
            currentTarget.TakeDamage(1000f);
        }

        yield return new WaitForSeconds(1f);

        Debug.Log("Волк исчезает после убийства");
        DestroyEntity();
    }

    private void MoveToBunker()
    {
        MoveTowardsWorld(Vector3.zero);

        if (Vector3.Distance(transform.position, Vector3.zero) < 20f || stateTimer > 20f)
        {
            currentState = WolfState.AttackingBunker;
            stateTimer = 0f;
            Debug.Log("Волк достиг бункера");
        }
    }

    private void AttackBunker()
    {
        // Вращаемся вокруг бункера
        transform.RotateAround(Vector3.zero, Vector3.forward, 90f * Time.deltaTime);

        if (stateTimer > 3f)
        {
            Debug.Log("Волк уничтожен электричеством бункера");
            TakeDamage(1000f);
        }
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        if (currentHealth <= 0)
        {
            currentState = WolfState.Dead;
            Debug.Log("Волк мертв");
        }
    }
}
