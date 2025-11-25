using UnityEngine;

public class Scanner : Entity
{
    private enum ScannerState { Moving, Scanning, Leaving }
    private ScannerState currentState = ScannerState.Moving;

    private SectorManager sectorManager;
    private int currentSector = 0;
    private float[] sectorAngles = { 0f, Mathf.PI / 2, Mathf.PI, 3 * Mathf.PI / 2 };
    private float scanRadius = 400f;
    private float timePerSector = 3f;
    private float sectorTimer;

    protected override void InitializeMovement()
    {
        entityColor = Color.blue;

        sectorManager = SectorManager.Instance;
        if (sectorManager != null)
        {
            sectorManager.RegisterScanner(this);
        }

        float spawnAngle = Random.Range(0f, 2f * Mathf.PI);
        polarPosition = new Vector2(800f, spawnAngle);

        currentState = ScannerState.Moving;
        sectorTimer = timePerSector;

        moveSpeed = 80f;

        Debug.Log($"Сканер создан. Начальный угол: {spawnAngle * Mathf.Rad2Deg:F1}°");
    }

    protected override void Move()
    {
        switch (currentState)
        {
            case ScannerState.Moving:
                MoveToSector();
                break;

            case ScannerState.Scanning:
                ScanSector();
                break;

            case ScannerState.Leaving:
                Leave();
                break;
        }
    }

    private void MoveToSector()
    {
        Vector2 targetPosition = new Vector2(scanRadius, sectorAngles[currentSector]);
        polarPosition = Vector2.MoveTowards(polarPosition, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(polarPosition, targetPosition) < 10f)
        {
            currentState = ScannerState.Scanning;
            sectorTimer = timePerSector;
            Debug.Log($"Сканер достиг сектора {currentSector + 1}");
        }
    }

    private void ScanSector()
    {
        sectorTimer -= Time.deltaTime;

        polarPosition = new Vector2(
            scanRadius,
            sectorAngles[currentSector] + Mathf.Sin(Time.time) * 0.1f
        );

        if (sectorTimer <= 0)
        {
            currentSector++;
            if (currentSector >= sectorAngles.Length)
            {
                currentState = ScannerState.Leaving;
                Debug.Log("Сканер завершил сканирование всех секторов");
            }
            else
            {
                currentState = ScannerState.Moving;
                Debug.Log($"Сканер перемещается к сектору {currentSector + 1}");
            }
        }
    }

    private void Leave()
    {
        Vector2 exitPosition = new Vector2(800f, polarPosition.y);
        polarPosition = Vector2.MoveTowards(polarPosition, exitPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(polarPosition, exitPosition) < 10f)
        {
            Debug.Log("Сканер покинул диск");
            DestroyEntity();
        }
    }

    protected override void OnDestroy()
    {
        if (sectorManager != null)
        {
            sectorManager.UnregisterScanner(this);
        }
        base.OnDestroy();
    }
}