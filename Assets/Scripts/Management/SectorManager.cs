using System.Collections.Generic;
using UnityEngine;

public class SectorManager : MonoBehaviour
{
    public static SectorManager Instance;

    [Header("Настройки секторов")]
    [SerializeField] private int sectorCount = 8;
    [SerializeField] private float maxSearchRadius = 200f;

    private float sectorAngleSize;

    // Списки всех сущностей
    private List<Entity> allNeutrals = new List<Entity>();
    private List<Scanner> allScanners = new List<Scanner>();

    // Последний уничтоженный сканер
    private Scanner lastDestroyedScanner;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        sectorAngleSize = (2f * Mathf.PI) / sectorCount;

        Debug.Log($"SectorManager инициализирован. Секторов: {sectorCount}, угол сектора: {sectorAngleSize * Mathf.Rad2Deg}°");
    }

    // ========== РЕГИСТРАЦИЯ НЕЙТРАЛОВ ==========

    /// <summary>
    /// Регистрирует нейтральную сущность (Олень, Енот)
    /// </summary>
    public void RegisterNeutral(Entity neutral)
    {
        if (neutral == null) return;

        if (!allNeutrals.Contains(neutral))
        {
            allNeutrals.Add(neutral);
            Debug.Log($"Зарегистрирован нейтрал: {neutral.name}");
        }
    }

    /// <summary>
    /// Удаляет нейтральную сущность из списка
    /// </summary>
    public void UnregisterNeutral(Entity neutral)
    {
        if (neutral == null) return;

        if (allNeutrals.Contains(neutral))
        {
            allNeutrals.Remove(neutral);
            Debug.Log($"Удалён нейтрал: {neutral.name}");
        }
    }

    // ========== РЕГИСТРАЦИЯ СКАНЕРОВ ==========

    /// <summary>
    /// Регистрирует сканер
    /// </summary>
    public void RegisterScanner(Scanner scanner)
    {
        if (scanner == null) return;

        if (!allScanners.Contains(scanner))
        {
            allScanners.Add(scanner);
            Debug.Log($"Зарегистрирован сканер: {scanner.name}");
        }
    }

    /// <summary>
    /// Удаляет сканер из списка и запоминает как последний уничтоженный
    /// </summary>
    public void UnregisterScanner(Scanner scanner)
    {
        if (scanner == null) return;

        if (allScanners.Contains(scanner))
        {
            allScanners.Remove(scanner);
            lastDestroyedScanner = scanner;
            Debug.Log($"Уничтожен сканер: {scanner.name}");
        }
    }

    /// <summary>
    /// Возвращает последний уничтоженный сканер
    /// </summary>
    public Scanner GetLastDestroyedScanner()
    {
        return lastDestroyedScanner;
    }

    // ========== ПОИСК В СЕКТОРЕ ==========

    /// <summary>
    /// Находит всех нейтралов в указанном секторе
    /// </summary>
    /// <param name="centerAngle">Центральный угол сектора в радианах</param>
    /// <param name="radius">Радиус поиска</param>
    /// <returns>Список нейтралов в секторе</returns>
    public List<Entity> GetNeutralsInSector(float centerAngle, float radius)
    {
        List<Entity> result = new List<Entity>();

        // Нормализуем угол 0-2π
        if (centerAngle < 0) centerAngle += 2f * Mathf.PI;
        if (centerAngle > 2f * Mathf.PI) centerAngle %= (2f * Mathf.PI);

        // Половина угла сектора
        float halfSector = sectorAngleSize * 0.5f;

        // Ищем всех нейтралов в радиусе
        foreach (Entity neutral in allNeutrals)
        {
            if (neutral == null) continue;

            Vector3 pos = neutral.transform.position;
            float distance = pos.magnitude;

            // Проверяем радиус
            if (distance > radius) continue;

            // Вычисляем угол нейтрала
            float neutralAngle = Mathf.Atan2(pos.y, pos.x);
            if (neutralAngle < 0) neutralAngle += 2f * Mathf.PI;

            // Разница углов
            float angleDiff = Mathf.Abs(Mathf.DeltaAngle(
                neutralAngle * Mathf.Rad2Deg,
                centerAngle * Mathf.Rad2Deg
            ));

            // Проверяем, находится ли в секторе
            if (angleDiff <= halfSector * Mathf.Rad2Deg)
            {
                result.Add(neutral);
            }
        }

        Debug.Log($"Найдено {result.Count} нейтралов в секторе {centerAngle * Mathf.Rad2Deg:F1}°");
        return result;
    }

    /// <summary>
    /// Находит все сканеры в указанном секторе
    /// </summary>
    public List<Scanner> GetScannersInSector(float centerAngle, float radius)
    {
        List<Scanner> result = new List<Scanner>();

        if (centerAngle < 0) centerAngle += 2f * Mathf.PI;
        float halfSector = sectorAngleSize * 0.5f;

        foreach (Scanner scanner in allScanners)
        {
            if (scanner == null) continue;

            Vector3 pos = scanner.transform.position;
            float distance = pos.magnitude;
            if (distance > radius) continue;

            float scannerAngle = Mathf.Atan2(pos.y, pos.x);
            if (scannerAngle < 0) scannerAngle += 2f * Mathf.PI;

            float angleDiff = Mathf.Abs(Mathf.DeltaAngle(
                scannerAngle * Mathf.Rad2Deg,
                centerAngle * Mathf.Rad2Deg
            ));

            if (angleDiff <= halfSector * Mathf.Rad2Deg)
            {
                result.Add(scanner);
            }
        }

        return result;
    }

    // ========== УТИЛИТЫ ==========

    /// <summary>
    /// Возвращает центр указанного сектора
    /// </summary>
    public float GetSectorCenterAngle(int sectorIndex)
    {
        return sectorIndex * sectorAngleSize + sectorAngleSize * 0.5f;
    }

    /// <summary>
    /// Возвращает номер сектора для указанных мировых координат
    /// </summary>
    public int GetSectorByWorldPosition(Vector3 worldPosition)
    {
        float angle = Mathf.Atan2(worldPosition.y, worldPosition.x);
        if (angle < 0) angle += 2f * Mathf.PI;

        return Mathf.FloorToInt(angle / sectorAngleSize);
    }

    /// <summary>
    /// Возвращает количество зарегистрированных сущностей
    /// </summary>
    public void GetEntityCounts(out int neutrals, out int scanners)
    {
        neutrals = allNeutrals.Count;
        scanners = allScanners.Count;
    }

    // ========== ОТЛАДКА ==========

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.yellow;

        // Рисуем линии секторов
        for (int i = 0; i < sectorCount; i++)
        {
            float angle = i * sectorAngleSize;
            Vector3 point = new Vector3(
                Mathf.Cos(angle) * maxSearchRadius,
                Mathf.Sin(angle) * maxSearchRadius,
                0
            );
            Gizmos.DrawLine(Vector3.zero, point);
        }

        // Рисуем всех нейтралов
        Gizmos.color = Color.green;
        foreach (Entity neutral in allNeutrals)
        {
            if (neutral != null)
            {
                Gizmos.DrawSphere(neutral.transform.position, 3f);
            }
        }

        // Рисуем все сканеры
        Gizmos.color = Color.blue;
        foreach (Scanner scanner in allScanners)
        {
            if (scanner != null)
            {
                Gizmos.DrawWireSphere(scanner.transform.position, 5f);
            }
        }
    }
}