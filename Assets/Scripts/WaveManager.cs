using UnityEngine;
using System.Collections;

// Wave Setup - will appear in inspector in an organized way
[System.Serializable]
public class WaveData
{
    public string name = "Wave 1";

    [Header("Descenders")]
    public int amountDescenders = 3;
    public GameObject descenderPrefab;

    [Header("SideToSide")]
    public int amountSideToSide = 0;
    public GameObject sideToSidePrefab;

    [Header("Stationary")]
    public int amountStationary = 0;
    public GameObject stationaryPrefab;

    [Space]
    public float spawnInterval = 0.5f;
}

public class WaveManager : MonoBehaviour
{
    [SerializeField] private WaveData[] waves;
    [SerializeField] private float timeBetweenWaves = 3f; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ExecuteWaves());
    }

    IEnumerator ExecuteWaves()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            yield return StartCoroutine(SpawnWaves(waves[i]));

            // Wait for the screen to be clear before proceed
            yield return new WaitUntil(() =>
                FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length == 0
            );

            if (i < waves.Length - 1)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }
        Debug.Log("=== VICTORY ===");
        // TO DO: show the victory screen later
    }

    IEnumerator SpawnWaves(WaveData wave)
    {
        // Descenders
        for (int i = 0; i < wave.amountDescenders; i++)
        {
            if (wave.descenderPrefab == null) break;
            float x = Random.Range(-6f, 6f);
            Instantiate(wave.descenderPrefab, new Vector3(x, 6.5f, 0), Quaternion.identity);
            yield return new WaitForSeconds(wave.spawnInterval);

        }

        // SideToSide
        for (int i = 0; i < wave.amountSideToSide; i++)
        {
            if (wave.sideToSidePrefab == null) break;
            float side = (i % 2 == 0) ? -6f : 6f; // alternates left/right
            float y = Random.Range(1f, 5f);
            Instantiate(wave.sideToSidePrefab, new Vector3(side, y, 0), Quaternion.identity);
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        // Stationary
        for (int i = 0; i < wave.amountStationary; i++)
        {
            if (wave.stationaryPrefab == null) break;
            // distributes horizontally in uniform way
            int total = wave.amountStationary;
            float x = Mathf.Lerp(-6f, 6f, total == 1 ? 0.5f : (float)i / (total - 1));
            Instantiate(wave.stationaryPrefab, new Vector3(x, 4.5f, 0), Quaternion.identity);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }
}
