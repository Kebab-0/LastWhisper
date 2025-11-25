using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawnManager : MonoBehaviour
{
    [Header("Neutral Entities")]
    [SerializeField] private GameObject raccoonPrefab;
    [SerializeField] private GameObject deerPrefab;

    [Header("Monster Entities")]
    [SerializeField] private GameObject mutantWolfPrefab;
    [SerializeField] private GameObject commonFreakPrefab;

    [Header("Machine Entities")]
    [SerializeField] private GameObject scannerPrefab;
    [SerializeField] private GameObject repressorPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float neutralSpawnInterval = 5f;
    [SerializeField] private float monsterSpawnInterval = 10f;
    [SerializeField] private float machineSpawnInterval = 15f;

    [SerializeField] private int maxNeutrals = 3;
    [SerializeField] private int maxMonsters = 2;
    [SerializeField] private int maxMachines = 2;

    [Header("World Settings")]
    [SerializeField] private float diskRadius = 800f;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private List<Entity> activeNeutrals = new List<Entity>();
    private List<Entity> activeMonsters = new List<Entity>();
    private List<Entity> activeMachines = new List<Entity>();

    private float neutralTimer;
    private float monsterTimer;
    private float machineTimer;

    private void Start()
    {
        Debug.Log("EntitySpawnManager запущен. Радиус диска: " + diskRadius);
        LogSpawnerStatus();
    }

    private void Update()
    {
        neutralTimer += Time.deltaTime;
        monsterTimer += Time.deltaTime;
        machineTimer += Time.deltaTime;

        if (neutralTimer >= neutralSpawnInterval && activeNeutrals.Count < maxNeutrals)
        {
            SpawnNeutral();
            neutralTimer = 0f;
        }

        if (monsterTimer >= monsterSpawnInterval && activeMonsters.Count < maxMonsters)
        {
            SpawnMonster();
            monsterTimer = 0f;
        }

        if (machineTimer >= machineSpawnInterval && activeMachines.Count < maxMachines)
        {
            SpawnMachine();
            machineTimer = 0f;
        }

        CleanupDestroyedEntities();
    }

    private void SpawnNeutral()
    {
        if (raccoonPrefab == null && deerPrefab == null)
        {
            Debug.LogError("Нет доступных префабов нейтралов!");
            return;
        }

        GameObject prefab = Random.Range(0, 2) == 0 ? raccoonPrefab : deerPrefab;
        if (prefab == null)
        {
            prefab = raccoonPrefab ?? deerPrefab;
        }

        var entity = SpawnEntityAtBorder(prefab, activeNeutrals);

        if (enableDebugLogs && entity != null)
        {
            Debug.Log($"Создан нейтрал: {entity.GetType().Name}. Всего нейтралов: {activeNeutrals.Count}");
        }
    }

    private void SpawnMonster()
    {
        GameObject prefab = Random.Range(0, 2) == 0 ? mutantWolfPrefab : commonFreakPrefab;
        if (prefab != null)
        {
            SpawnEntityAtBorder(prefab, activeMonsters);
        }
    }

    private void SpawnMachine()
    {
        GameObject prefab = Random.Range(0, 2) == 0 ? scannerPrefab : repressorPrefab;
        if (prefab != null)
        {
            SpawnEntityAtBorder(prefab, activeMachines);
        }
    }

    private Entity SpawnEntityAtBorder(GameObject prefab, List<Entity> entityList)
    {
        if (prefab == null)
        {
            Debug.LogError("Попытка создать сущность из null префаба!");
            return null;
        }

        Vector2 spawnPolarPosition = new Vector2(
            diskRadius,
            Random.Range(0f, 2f * Mathf.PI)
        );

        Vector3 spawnWorldPosition = CoordinateConverter.PolarToWorld2D(spawnPolarPosition);
        GameObject entityObject = Instantiate(prefab, spawnWorldPosition, Quaternion.identity);
        Entity entity = entityObject.GetComponent<Entity>();

        if (entity != null)
        {
            entityList.Add(entity);
            Debug.Log($"Создана сущность на радиусе {diskRadius}, позиция: {spawnWorldPosition}");
            return entity;
        }
        else
        {
            Debug.LogError($"Созданный объект {prefab.name} не имеет компонента Entity!");
            return null;
        }
    }

    private void CleanupDestroyedEntities()
    {
        activeNeutrals.RemoveAll(entity => entity == null);
        activeMonsters.RemoveAll(entity => entity == null);
        activeMachines.RemoveAll(entity => entity == null);
    }

    private void LogSpawnerStatus()
    {
        Debug.Log("=== СТАТУС СПАВНЕРА ===");
        Debug.Log($"Радиус диска: {diskRadius}");
        Debug.Log("========================");
    }

    [ContextMenu("Validate All Prefabs")]
    public void ValidateAllPrefabs()
    {
        Debug.Log("=== ПРОВЕРКА ВСЕХ ПРЕФАБОВ ===");

        CheckPrefab(raccoonPrefab, "Енот");
        CheckPrefab(deerPrefab, "Олень");
        CheckPrefab(mutantWolfPrefab, "Волк мутированный");
        CheckPrefab(commonFreakPrefab, "Урод обыкновенный");
        CheckPrefab(scannerPrefab, "Сканер");
        CheckPrefab(repressorPrefab, "Репрессор");

        Debug.Log("=== ПРОВЕРКА ЗАВЕРШЕНА ===");
    }

    [ContextMenu("Spawn All Entities Test")]
    public void SpawnAllEntitiesTest()
    {
        Debug.Log("=== ТЕСТОВЫЙ СПАВН ВСЕХ СУЩНОСТЕЙ ===");

        if (raccoonPrefab != null) SpawnEntityAtBorder(raccoonPrefab, activeNeutrals);
        if (deerPrefab != null) SpawnEntityAtBorder(deerPrefab, activeNeutrals);
        if (mutantWolfPrefab != null) SpawnEntityAtBorder(mutantWolfPrefab, activeMonsters);
        if (commonFreakPrefab != null) SpawnEntityAtBorder(commonFreakPrefab, activeMonsters);
        if (scannerPrefab != null) SpawnEntityAtBorder(scannerPrefab, activeMachines);
        if (repressorPrefab != null) SpawnEntityAtBorder(repressorPrefab, activeMachines);

        Debug.Log($"Всего сущностей: Нейтралы={activeNeutrals.Count}, Монстры={activeMonsters.Count}, Машины={activeMachines.Count}");
    }

    [ContextMenu("Destroy All Entities")]
    public void DestroyAllEntities()
    {
        DestroyEntitiesInList(activeNeutrals);
        DestroyEntitiesInList(activeMonsters);
        DestroyEntitiesInList(activeMachines);

        Debug.Log("Все сущности уничтожены");
    }

    [ContextMenu("Spawn Test Neutral")]
    public void SpawnTestNeutral()
    {
        GameObject prefab = raccoonPrefab != null ? raccoonPrefab : deerPrefab;
        if (prefab != null)
        {
            SpawnEntityAtBorder(prefab, activeNeutrals);
            Debug.Log($"Создан тестовый нейтрал: {prefab.name}");
        }
        else
        {
            Debug.LogError("Нет доступных префабов нейтралов для теста!");
        }
    }

    [ContextMenu("Log Entities Status")]
    public void LogEntitiesStatus()
    {
        Debug.Log($"Текущие сущности: Нейтралы={activeNeutrals.Count}, Монстры={activeMonsters.Count}, Машины={activeMachines.Count}");
    }

    private void CheckPrefab(GameObject prefab, string name)
    {
        if (prefab == null)
        {
            Debug.LogError($"{name}: префаб не назначен!");
            return;
        }

        bool hasEntity = prefab.GetComponent<Entity>() != null;
        bool hasSpriteRenderer = prefab.GetComponent<SpriteRenderer>() != null;

        string status = hasEntity ? "OK" : "ERROR";
        Debug.Log($"{name}: {status} (Entity={hasEntity}, SpriteRenderer={hasSpriteRenderer})");

        if (!hasEntity) Debug.LogError($"{name}: отсутствует компонент Entity!");
    }

    private void DestroyEntitiesInList(List<Entity> entities)
    {
        foreach (Entity entity in entities)
        {
            if (entity != null)
                Destroy(entity.gameObject);
        }
        entities.Clear();
    }
}