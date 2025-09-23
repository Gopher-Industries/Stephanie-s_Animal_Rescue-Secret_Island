using UnityEditor;
using UnityEngine;

public class StarAmbientSpawner : MonoBehaviour
{
    [Header("Ambient Shooting Star")]
    [SerializeField] private GameObject shootingStarPrefab;
    [SerializeField] private float minShootingStarInterval = 25f;
    [SerializeField] private float maxShootingStarInterval = 40f;

    [Header("Ambient Static Stars")]
    [SerializeField] private GameObject staticStarPrefab;
    [SerializeField] private int staticStarCount = 20;

    private Camera mainCamera;

    // Timers for shooting stars
    private float lastShootingStarSpawnTime;
    private float nextShootingStarInterval;

    void Start()
    {
        mainCamera = Camera.main;
#if UNITY_EDITOR
        if (shootingStarPrefab == null)
        {
            Debug.LogError("You forgot the Shooting Star Particle prefab in the Inspector!", this);
        }

        if (staticStarPrefab == null)
        {
            Debug.LogError("You forgot the Ambient Static Star prefab in the Inspector!", this);
        }
#endif
        // Initialize timers to prevent instant spawns
        lastShootingStarSpawnTime = Time.time;
        nextShootingStarInterval = Random.Range(minShootingStarInterval, maxShootingStarInterval);

        if (staticStarPrefab != null)
        {
            SpawnStaticStars();
        }
    }


    void Update()
    {
        // Shooting Star Logic 
        if (Time.time - lastShootingStarSpawnTime >= nextShootingStarInterval)
        {
            Instantiate(shootingStarPrefab, GetRandomScreenPosition(), Quaternion.identity);
            nextShootingStarInterval = Random.Range(minShootingStarInterval, maxShootingStarInterval);
            lastShootingStarSpawnTime = Time.time;
        }
    }

    private Vector3 GetRandomScreenPosition()
    {
        // get random position on screen
        float screenX = Random.Range(0, Screen.width);
        float screenY = Random.Range(0, Screen.height);

        // Convert the random screen position to a world position using cameras z depth
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenX, screenY, mainCamera.nearClipPlane + 10));
        // discard z depth since this scene is 2D
        worldPosition.z = 0;

        return worldPosition;
    }

    private void SpawnStaticStars()
    {
        for (int i = 0; i < staticStarCount; i++)
        {
            Vector3 randomPosition = GetRandomScreenPosition();
            Instantiate(staticStarPrefab, randomPosition, Quaternion.identity);
        }
    }
}