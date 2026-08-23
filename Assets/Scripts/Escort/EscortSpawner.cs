using System.Collections;
using UnityEngine;

public class EscortSpawner : MonoBehaviour
{
    [SerializeField] private Vector2 spawnPosition;
    [SerializeField] private int spawnCount = 3;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Direction startDirection = Direction.Right;
    [SerializeField] private GameObject escortPrefab;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnOne()
    {
        if (escortPrefab == null)
        {
            Debug.LogWarning("EscortSpawner: escortPrefab is not assigned.", this);
            return;
        }

        GameObject instance = Instantiate(escortPrefab, spawnPosition, Quaternion.identity);
        EscortTarget escort = instance.GetComponent<EscortTarget>();
        if (escort != null)
        {
            escort.baseMoveSpeed = moveSpeed;
            escort.currentDirection = startDirection;
        }
        else
        {
            Debug.LogWarning("EscortSpawner: escortPrefab is missing an EscortTarget component.", this);
        }
    }
}
