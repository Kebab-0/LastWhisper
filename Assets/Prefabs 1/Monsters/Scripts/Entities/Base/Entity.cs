using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Base Characteristics")]
    [SerializeField] protected float health = 100f;
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float temperature = 20f;
    [SerializeField] protected float moveSpeed = 100f;
    [SerializeField] protected AudioClip movementSound;
    [SerializeField] protected Color entityColor = Color.white;

    protected float currentHealth;
    protected Vector2 polarPosition;
    protected AudioSource audioSource;
    protected SpriteRenderer spriteRenderer;

    protected const float WORLD_SCALE = 1f;
    protected const float ENTITY_SCALE = 10f;

    public float Health => currentHealth;
    public float Damage => damage;
    public Vector2 PolarPosition => polarPosition;

    protected virtual void Awake()
    {
        currentHealth = health;
        CreateRequiredComponents();
        InitializeComponents();
        InitializeMovement();
    }

    protected virtual void Update()
    {
        Move();
        UpdateWorldPosition();
    }

    protected virtual void CreateRequiredComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    protected virtual void InitializeComponents()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = entityColor;
            transform.localScale = Vector3.one * ENTITY_SCALE;

            if (spriteRenderer.sprite == null)
            {
                CreateDefaultSprite();
            }
        }
    }

    protected virtual void CreateDefaultSprite()
    {
        Texture2D texture = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = entityColor;
        }
        texture.SetPixels(pixels);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = sprite;
    }

    protected abstract void InitializeMovement();
    protected abstract void Move();

    protected void UpdateWorldPosition()
    {
        transform.position = CoordinateConverter.PolarToWorld2D(polarPosition) * WORLD_SCALE;
    }

    protected virtual void PlayMovementSound()
    {
        if (movementSound != null && audioSource != null)
        {
            audioSource.clip = movementSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            DestroyEntity();
    }

    protected virtual void DestroyEntity()
    {
        Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
    }
}