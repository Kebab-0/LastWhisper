using UnityEngine;

public class CommonFreak : Entity
{
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float freezeDuration = 5f;
    [SerializeField] private float lifetimeBeforeExit = 60f;
    private Vector3 moveDirection;
    private float directionChangeTimer;
    private float directionChangeInterval = 2f;
    private float maxWanderRadius = 120f;
    private float freezeTimer;
    private float lifeTimer;
    private Transform currentTarget;
    private bool isFreezing;
    private bool isExiting;

    protected override bool UsePolarMovement => false;

    protected override void InitializeMovement()
    {
        entityColor = Color.magenta;

        // Начальное направление
        moveDirection = Random.insideUnitCircle.normalized;
        moveDirection.z = 0;

        // Скорость (можно переопределить в инспекторе)
        EnsureMoveSpeed(40f);
    }

    protected override void Move()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifetimeBeforeExit)
        {
            isExiting = true;
        }

        if (isExiting)
        {
            MoveInDirectionWorld(transform.position.normalized);
            return;
        }

        if (isFreezing)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f)
            {
                isFreezing = false;
                directionChangeTimer = directionChangeInterval; // сменим направление при выходе из заморозки
            }

            return;
        }

        KillEntitiesInDetectionRadius();
        AcquireTarget();

        if (currentTarget != null)
        {
            // Приоритетное убийство найденной цели
            ChaseTarget();
            return;
        }

        directionChangeTimer += Time.deltaTime;

        // Меняем направление через интервал
        if (directionChangeTimer >= directionChangeInterval)
        {
            moveDirection = Random.insideUnitCircle.normalized;
            moveDirection.z = 0;
            directionChangeTimer = 0f;
        }

        // Двигаемся
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Не выходим за рабочий радиус
        float distanceFromCenter = transform.position.magnitude;
        if (distanceFromCenter > maxWanderRadius)
        {
            // Возвращаемся к центру
            moveDirection = -transform.position.normalized;
        }

        // Случайные повороты
        if (Random.value < 0.01f)
        {
            float randomAngle = Random.Range(-45f, 45f);
            moveDirection = Quaternion.Euler(0, 0, randomAngle) * moveDirection;
        }
    }

    private void AcquireTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        float bestDistance = detectionRadius + 1f;
        currentTarget = null;

        foreach (Collider2D hit in hits)
        {
            Entity entity = hit.GetComponent<Entity>();

            if (entity is Deer || entity is Raccoon)
            {
                float distance = Vector3.Distance(transform.position, entity.transform.position);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    currentTarget = entity.transform;
                }
            }
        }
    }

    private void ChaseTarget()
    {
        if (currentTarget == null) return;

        Vector3 targetPosition = currentTarget.position;
        float distance = Vector3.Distance(transform.position, targetPosition);

        MoveTowardsWorld(targetPosition);

        if (distance <= 1f)
        {
            Entity targetEntity = currentTarget.GetComponent<Entity>();

            if (targetEntity != null)
            {
                targetEntity.TakeDamage(float.MaxValue); // мгновенное убийство
            }

            currentTarget = null;
            isFreezing = true;
            freezeTimer = freezeDuration;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // При столкновении с игроком или другой сущностью
        Entity otherEntity = other.GetComponent<Entity>();
        if (otherEntity != null && otherEntity != this)
        {
            // Наносим урон
            otherEntity.TakeDamage(damage);

            // Отскакиваем
            moveDirection = -moveDirection;
        }
    }

    private void KillEntitiesInDetectionRadius()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (Collider2D hit in hits)
        {
            Entity entity = hit.GetComponent<Entity>();

            if (entity != null && entity != this)
            {
                entity.TakeDamage(float.MaxValue);
            }
        }
    }
}
