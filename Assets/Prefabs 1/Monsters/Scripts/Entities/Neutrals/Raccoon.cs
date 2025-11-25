using UnityEngine;

public class Raccoon : Entity
{
    private enum Direction { Left, Right }
    private Direction moveDirection;
    private Vector2 startPolarPosition;
    private Vector2 targetPolarPosition;
    private bool hasReachedTarget = false;
    private float journeyTime = 15f;
    private float startTime;

    protected override void InitializeMovement()
    {
        entityColor = new Color(0.5f, 0.5f, 0.5f);

        // Появление на границе
        float spawnAngle = Random.Range(0f, 2f * Mathf.PI);
        startPolarPosition = new Vector2(800f, spawnAngle);
        polarPosition = startPolarPosition;

        // Выбор направления движения
        moveDirection = Random.Range(0, 2) == 0 ? Direction.Left : Direction.Right;

        // Целевая точка - противоположная сторона диска
        float targetAngle;
        if (moveDirection == Direction.Left)
        {
            targetAngle = (spawnAngle - Mathf.PI) % (2f * Mathf.PI);
        }
        else
        {
            targetAngle = (spawnAngle + Mathf.PI) % (2f * Mathf.PI);
        }

        // Целевая точка на противоположной стороне
        targetPolarPosition = new Vector2(800f, targetAngle);

        startTime = Time.time;
        moveSpeed = 80f;

        Debug.Log($"Енот: появился под углом {spawnAngle * Mathf.Rad2Deg:F1}°, цель: {targetAngle * Mathf.Rad2Deg:F1}°");
    }

    protected override void Move()
    {
        if (hasReachedTarget) return;

        float progress = (Time.time - startTime) / journeyTime;

        if (progress >= 1f)
        {
            hasReachedTarget = true;
            Debug.Log("Енот достиг цели и исчезает");
            DestroyEntity();
            return;
        }

        // Сложное движение через весь диск
        // Сначала движемся к центру, затем к целевой точке

        // Изменяем радиус - сначала уменьшаем, потом увеличиваем
        float radiusProgress;
        if (progress < 0.3f)
        {
            // Фаза 1: движение от границы к центру
            radiusProgress = progress / 0.3f;
            float currentRadius = Mathf.Lerp(800f, 200f, radiusProgress);

            // Плавное изменение угла
            float currentAngle = Mathf.LerpAngle(startPolarPosition.y, startPolarPosition.y + (moveDirection == Direction.Left ? -0.5f : 0.5f), radiusProgress);

            polarPosition = new Vector2(currentRadius, currentAngle);
        }
        else if (progress < 0.7f)
        {
            // Фаза 2: движение через центр с изменением направления
            radiusProgress = (progress - 0.3f) / 0.4f;
            float currentRadius = Mathf.Lerp(200f, 200f, radiusProgress); // Остаемся вблизи центра

            // Значительное изменение угла
            float angleProgress = (progress - 0.3f) / 0.4f;
            float startMidAngle = startPolarPosition.y + (moveDirection == Direction.Left ? -0.5f : 0.5f);
            float endMidAngle = targetPolarPosition.y + (moveDirection == Direction.Left ? 0.5f : -0.5f);
            float currentAngle = Mathf.LerpAngle(startMidAngle, endMidAngle, angleProgress);

            polarPosition = new Vector2(currentRadius, currentAngle);
        }
        else
        {
            // Фаза 3: движение от центра к границе
            radiusProgress = (progress - 0.7f) / 0.3f;
            float currentRadius = Mathf.Lerp(200f, 800f, radiusProgress);

            // Плавное движение к целевому углу
            float startFinalAngle = targetPolarPosition.y + (moveDirection == Direction.Left ? 0.5f : -0.5f);
            float currentAngle = Mathf.LerpAngle(startFinalAngle, targetPolarPosition.y, radiusProgress);

            polarPosition = new Vector2(currentRadius, currentAngle);
        }

        // Отладочная информация
        if (Mathf.FloorToInt(Time.time) % 5 == 0 && Mathf.FloorToInt(Time.time) != Mathf.FloorToInt(Time.time - Time.deltaTime))
        {
            Debug.Log($"Енот: прогресс {progress:P0}, радиус {polarPosition.x:F0}, угол {polarPosition.y * Mathf.Rad2Deg:F1}°");
        }
    }
}