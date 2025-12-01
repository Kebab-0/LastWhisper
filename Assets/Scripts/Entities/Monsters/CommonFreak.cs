using UnityEngine;

public class CommonFreak : Entity
{
    private Vector3 moveDirection;
    private float directionChangeTimer;
    private float directionChangeInterval = 2f;
    private float maxWanderRadius = 120f;

    protected override bool UsePolarMovement => false;

    protected override void InitializeMovement()
    {
        entityColor = Color.magenta;

        // Начальное направление
        moveDirection = Random.insideUnitCircle.normalized;
        moveDirection.z = 0;

        // Скорость
            moveSpeed = 40f;
    }

    protected override void Move()
    {
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
}