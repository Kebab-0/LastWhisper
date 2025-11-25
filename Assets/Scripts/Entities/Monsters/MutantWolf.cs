using System.Collections;
using UnityEngine;

public class MutantWolf : Entity
{
    private enum WolfState { SearchingFirstSector, MovingToFirstSector, SearchingSecondSector, MovingToSecondSector, ChasingNeutral, Killing, MovingToBunker, AttackingBunker, Dead }
    private WolfState currentState = WolfState.SearchingFirstSector;

    private SectorManager sectorManager;
    private Entity currentTarget;
    private Vector2 firstSectorCenter;
    private Vector2 secondSectorCenter;
    private Vector2 currentTargetPosition;
    private bool hasFoundTarget = false;
    private float killRange = 50f;
    private float searchRadius = 200f;

    protected override void InitializeMovement()
    {
        entityColor = Color.red;

        sectorManager = SectorManager.Instance;

        // Появление на случайной граничной точке
        float spawnAngle = Random.Range(0f, 2f * Mathf.PI);
        polarPosition = new Vector2(800f, spawnAngle);

        // Определяем центры секторов для поиска
        firstSectorCenter = new Vector2(600f, spawnAngle); // Первый сектор - ближе к точке появления
        secondSectorCenter = new Vector2(300f, (spawnAngle + Mathf.PI) % (2f * Mathf.PI)); // Второй сектор - противоположная сторона

        currentState = WolfState.MovingToFirstSector;
        currentTargetPosition = firstSectorCenter;

        moveSpeed = 80f; // Нормальная скорость

        Debug.Log($"Волк создан. Движется к первому сектору: радиус {firstSectorCenter.x}, угол {firstSectorCenter.y * Mathf.Rad2Deg:F1}°");
    }

    protected override void Move()
    {
        if (currentState == WolfState.Dead) return;

        switch (currentState)
        {
            case WolfState.MovingToFirstSector:
                MoveToPosition(firstSectorCenter, WolfState.SearchingFirstSector);
                break;

            case WolfState.SearchingFirstSector:
                SearchForNeutral(firstSectorCenter, WolfState.MovingToSecondSector);
                break;

            case WolfState.MovingToSecondSector:
                MoveToPosition(secondSectorCenter, WolfState.SearchingSecondSector);
                break;

            case WolfState.SearchingSecondSector:
                SearchForNeutral(secondSectorCenter, WolfState.MovingToBunker);
                break;

            case WolfState.ChasingNeutral:
                ChaseNeutral();
                break;

            case WolfState.MovingToBunker:
                MoveToBunker();
                break;

            case WolfState.AttackingBunker:
                // Атака происходит в корутине
                break;

            case WolfState.Killing:
                // Убийство происходит в корутине
                break;
        }
    }

    private void MoveToPosition(Vector2 targetPosition, WolfState nextState)
    {
        // Плавное движение к целевой позиции
        polarPosition = Vector2.MoveTowards(polarPosition, targetPosition, moveSpeed * Time.deltaTime);

        // Проверяем достижение цели
        if (Vector2.Distance(polarPosition, targetPosition) < 10f)
        {
            currentState = nextState;
            Debug.Log($"Волк достиг позиции: радиус {targetPosition.x}, угол {targetPosition.y * Mathf.Rad2Deg:F1}°");
        }
    }

    private void SearchForNeutral(Vector2 searchCenter, WolfState nextStateIfNoTarget)
    {
        // Ищем нейтралов в текущем секторе
        var neutrals = sectorManager.GetNeutralsInSector(searchCenter.y, searchCenter.x);

        if (neutrals.Count > 0 && !hasFoundTarget)
        {
            // Нашли нейтрала - начинаем преследование
            currentTarget = neutrals[0];
            hasFoundTarget = true;
            currentState = WolfState.ChasingNeutral;
            Debug.Log($"Волк нашел нейтрала: {currentTarget.GetType().Name}");
        }
        else
        {
            // Не нашли нейтрала - переходим к следующему состоянию
            currentState = nextStateIfNoTarget;

            if (nextStateIfNoTarget == WolfState.MovingToSecondSector)
            {
                Debug.Log("Волк не нашел нейтралов в первом секторе, движется ко второму");
                currentTargetPosition = secondSectorCenter;
            }
            else if (nextStateIfNoTarget == WolfState.MovingToBunker)
            {
                Debug.Log("Волк не нашел нейтралов ни в одном секторе, атакует бункер");
            }
        }

        // Небольшое движение на месте во время поиска
        float wanderRadius = 20f;
        polarPosition = new Vector2(
            searchCenter.x + Mathf.Sin(Time.time * 2f) * wanderRadius,
            searchCenter.y + Mathf.Cos(Time.time * 2f) * wanderRadius * 0.01f
        );
    }

    private void ChaseNeutral()
    {
        if (currentTarget == null)
        {
            // Цель исчезла - возвращаемся к поиску
            hasFoundTarget = false;
            currentState = WolfState.SearchingFirstSector;
            return;
        }

        // Двигаемся к позиции нейтрала
        Vector2 targetPosition = currentTarget.PolarPosition;
        polarPosition = Vector2.MoveTowards(polarPosition, targetPosition, moveSpeed * Time.deltaTime);

        // Проверяем достижение цели
        if (Vector2.Distance(polarPosition, targetPosition) < killRange)
        {
            StartCoroutine(KillNeutral());
        }
    }

    private void MoveToBunker()
    {
        // Двигаемся к бункеру (центру диска)
        Vector2 bunkerPosition = new Vector2(0f, 0f);
        polarPosition = Vector2.MoveTowards(polarPosition, bunkerPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(polarPosition, bunkerPosition) < 100f)
        {
            StartCoroutine(AttackBunker());
        }
    }

    private IEnumerator KillNeutral()
    {
        currentState = WolfState.Killing;

        if (currentTarget != null)
        {
            Debug.Log($"Волк убивает {currentTarget.GetType().Name}");
            currentTarget.TakeDamage(1000f); // Мгновенная смерть

            // Ждем некоторое время после убийства
            yield return new WaitForSeconds(2f);

            Debug.Log("Волк удаляется после убийства");
            DestroyEntity();
        }
        else
        {
            // Если цель исчезла, просто удаляемся
            DestroyEntity();
        }
    }

    private IEnumerator AttackBunker()
    {
        currentState = WolfState.AttackingBunker;
        Debug.Log("Волк атакует бункер");

        // Атака бункера в течение 2 секунд
        yield return new WaitForSeconds(2f);

        // Бункер поражает волка электричеством
        Debug.Log("Бункер поражает волка электричеством");
        TakeDamage(1000f);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        if (currentHealth <= 0)
        {
            currentState = WolfState.Dead;
            Debug.Log("Волк убит электричеством бункера");
        }
    }
}