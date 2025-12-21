using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Основные характеристики")]
    [SerializeField] protected float health = 100f;
    [SerializeField] protected float damage = 10f;
    [SerializeField, Tooltip("Скорость перемещения. Если не задано, возьмется значение по умолчанию для конкретного типа сущности.")]
    protected float moveSpeed = 0f;
    [SerializeField] protected AudioClip movementSound;
    [SerializeField] protected Color entityColor = Color.white;
    [SerializeField] protected Material entityMaterial; // Материал для 3D объектов

    // Радиусы (константы)
    public const float WORK_RADIUS = 160f;
    public const float SPAWN_RADIUS = 170f;
    public const float DESPAWN_RADIUS = 200f;

    // Компоненты
    protected float currentHealth;
    protected Vector2 polarPosition;
    protected AudioSource audioSource;
    protected MeshRenderer meshRenderer; // Изменено с SpriteRenderer на MeshRenderer
    protected MeshFilter meshFilter;

    // Публичные свойства
    public float Health => currentHealth;
    public float Damage => damage;
    public float MoveSpeed => moveSpeed;
    public Vector2 PolarPosition => polarPosition;

    // Определяет, использует ли сущность полярное движение
    protected virtual bool UsePolarMovement => true;

    // ========== LIFECYCLE ==========

    protected virtual void Awake()
    {
        currentHealth = health;
        InitializeComponents();
        SpawnOnCircle();
    }

    protected virtual void Start()
    {
        RegisterInSectorManager();
        InitializeMovement();
    }

    protected virtual void Update()
    {
        Move();

        if (UsePolarMovement)
            UpdateWorldPosition();

        CheckDespawn();
    }

    // ========== ИНИЦИАЛИЗАЦИЯ КОМПОНЕНТОВ (3D ВЕРСИЯ) ==========

    private void InitializeComponents()
    {
        // 1. MeshRenderer для 3D объектов
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            // Проверяем, есть ли дочерние объекты с MeshRenderer
            meshRenderer = GetComponentInChildren<MeshRenderer>();

            if (meshRenderer == null)
            {
                // Создаем простой 3D куб
                CreateDefault3DObject();
            }
        }

        // 2. MeshFilter
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = GetComponentInChildren<MeshFilter>();
        }

        // 3. AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 4. Устанавливаем цвет/материал
        if (meshRenderer != null)
        {
            if (entityMaterial != null)
            {
                meshRenderer.material = entityMaterial;
            }
            else
            {
                // Создаем простой материал с цветом
                Material newMat = new Material(Shader.Find("Standard"));
                newMat.color = entityColor;
                meshRenderer.material = newMat;
            }
        }
    }

    private void CreateDefault3DObject()
    {
        // Создаем простой куб
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.SetParent(transform);
        cube.transform.localPosition = Vector3.zero;
        cube.transform.localScale = Vector3.one;

        // Получаем компоненты
        meshRenderer = cube.GetComponent<MeshRenderer>();
        meshFilter = cube.GetComponent<MeshFilter>();

        // Устанавливаем материал
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = entityColor;
        meshRenderer.material = mat;

        Debug.Log($"{name}: Создан 3D куб");
    }

    // ========== РЕГИСТРАЦИЯ В SECTOR MANAGER ==========

    private void RegisterInSectorManager()
    {
        if (SectorManager.Instance == null) return;

        if (this is Deer || this is Raccoon)
        {
            SectorManager.Instance.RegisterNeutral(this);
        }
        else if (this is Scanner)
        {
            SectorManager.Instance.RegisterScanner(this as Scanner);
        }
    }

    private void UnregisterFromSectorManager()
    {
        if (SectorManager.Instance == null) return;

        if (this is Deer || this is Raccoon)
        {
            SectorManager.Instance.UnregisterNeutral(this);
        }
        else if (this is Scanner)
        {
            SectorManager.Instance.UnregisterScanner(this as Scanner);
        }
    }

    // ========== СПАВН И ДЕСПАВН ==========

    private void SpawnOnCircle()
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        polarPosition = new Vector2(SPAWN_RADIUS, angle);

        if (!UsePolarMovement)
        {
            transform.position = CoordinateConverter.PolarToWorld2D(polarPosition);
        }
    }

    private void CheckDespawn()
    {
        float distance = Vector3.Distance(transform.position, Vector3.zero);

        if (distance > DESPAWN_RADIUS)
        {
            DestroyEntity();
        }
    }

    // ========== ДВИЖЕНИЕ ==========

    protected abstract void InitializeMovement();
    protected abstract void Move();

    protected void UpdateWorldPosition()
    {
        transform.position = CoordinateConverter.PolarToWorld2D(polarPosition);
    }

    // ========== УТИЛИТЫ ДЛЯ ДВИЖЕНИЯ ==========

    protected void MoveTowardsWorld(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );
    }

    protected void EnsureMoveSpeed(float defaultSpeed)
    {
        if (moveSpeed <= 0.001f)
        {
            moveSpeed = defaultSpeed;
        }
    }

    protected void MoveTowardsPolar(Vector2 target)
    {
        polarPosition = Vector2.MoveTowards(
            polarPosition,
            target,
            moveSpeed * Time.deltaTime
        );
    }

    protected void MoveInDirectionWorld(Vector3 direction)
    {
        transform.position += direction.normalized * moveSpeed * Time.deltaTime;
    }

    // ========== ЗВУКИ ==========

    protected virtual void PlayMovementSound()
    {
        if (movementSound != null && audioSource != null && !audioSource.isPlaying)
        {
            audioSource.clip = movementSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    protected virtual void StopMovementSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // ========== УРОН И СМЕРТЬ ==========

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            DestroyEntity();
        }
    }

    protected virtual void DestroyEntity()
    {
        UnregisterFromSectorManager();
        Destroy(gameObject);
    }

    // ========== УТИЛИТЫ ДЛЯ КООРДИНАТ ==========

    protected Vector3 GetRandomPointOnCircle(float radius)
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        return new Vector3(
            Mathf.Cos(angle) * radius,
            0,
            Mathf.Sin(angle) * radius
        );
    }

    protected Vector3 GetRandomPointInCircle(float maxRadius)
    {
        Vector2 random = Random.insideUnitCircle * maxRadius;
        return new Vector3(random.x, 0, random.y);
    }

    protected Vector3 GetOppositePoint(float radius, Vector3 currentPosition)
    {
        Vector3 direction = currentPosition.normalized;
        return -direction * radius;
    }
}
