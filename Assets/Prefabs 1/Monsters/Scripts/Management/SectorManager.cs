using System.Collections.Generic;
using UnityEngine;

public class SectorManager : MonoBehaviour
{
    [Header("Sector Settings")]
    [SerializeField] private int angularSectors = 8;
    [SerializeField] private float sectorCheckRadius = 150f;

    private List<Entity>[] entitySectors;
    private List<Scanner> activeScanners = new List<Scanner>();
    private List<Entity> allNeutrals = new List<Entity>();

    public static SectorManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        entitySectors = new List<Entity>[angularSectors];
        for (int i = 0; i < angularSectors; i++)
        {
            entitySectors[i] = new List<Entity>();
        }
    }

    public void RegisterEntity(Entity entity)
    {
        Vector2 polarPos = CoordinateConverter.WorldToPolar2D(entity.transform.position);
        int sector = GetSectorIndex(polarPos.y);

        if (!entitySectors[sector].Contains(entity))
        {
            entitySectors[sector].Add(entity);

            // Отслеживаем нейтралов отдельно
            if (entity is Raccoon || entity is Deer)
            {
                if (!allNeutrals.Contains(entity))
                {
                    allNeutrals.Add(entity);
                }
            }
        }
    }

    public void UpdateEntityPosition(Entity entity)
    {
        UnregisterEntity(entity);
        RegisterEntity(entity);
    }

    public void UnregisterEntity(Entity entity)
    {
        foreach (var sector in entitySectors)
        {
            sector.Remove(entity);
        }

        // Удаляем из списка нейтралов
        if (entity is Raccoon || entity is Deer)
        {
            allNeutrals.Remove(entity);
        }
    }

    public List<Entity> GetNeutralsInSector(float angle, float searchRadius)
    {
        int sector = GetSectorIndex(angle);
        List<Entity> neutralsInRange = new List<Entity>();

        // Ищем нейтралов в указанном секторе и радиусе
        foreach (Entity neutral in allNeutrals)
        {
            if (neutral == null) continue;

            Vector2 neutralPolarPos = neutral.PolarPosition;
            int neutralSector = GetSectorIndex(neutralPolarPos.y);

            // Проверяем, что нейтрал в нужном секторе и в пределах радиуса поиска
            if (neutralSector == sector)
            {
                float distance = Mathf.Abs(neutralPolarPos.x - searchRadius);
                if (distance < sectorCheckRadius)
                {
                    neutralsInRange.Add(neutral);
                }
            }
        }

        return neutralsInRange;
    }

    public int GetSectorIndex(float angle)
    {
        float normalizedAngle = angle % (2 * Mathf.PI);
        if (normalizedAngle < 0) normalizedAngle += 2 * Mathf.PI;

        return Mathf.FloorToInt(normalizedAngle / (2 * Mathf.PI) * angularSectors);
    }

    public float GetSectorCenterAngle(int sectorIndex)
    {
        return (sectorIndex * 2 * Mathf.PI / angularSectors) + (Mathf.PI / angularSectors);
    }

    public void RegisterScanner(Scanner scanner)
    {
        if (!activeScanners.Contains(scanner))
            activeScanners.Add(scanner);
    }

    public void UnregisterScanner(Scanner scanner)
    {
        activeScanners.Remove(scanner);
    }

    public Scanner GetLastDestroyedScanner()
    {
        return activeScanners.Count > 0 ? activeScanners[activeScanners.Count - 1] : null;
    }
}