using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObstacleAgent : MonoBehaviour
{
    [SerializeField]
    private float carvingTime = 0.5f;
    [SerializeField]
    private float carvingMoveThreshold = 0.1f;

    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private NavMeshObstacle obstacle;
    public NavMeshObstacle Obstacle { get { return obstacle; } }

    private float lastMoveTime;
    private Vector3 lastPost;

    private void Awake()
    {
        obstacle.enabled = false;
        obstacle.carveOnlyStationary = false;
        obstacle.carving = true;

        lastPost = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(lastPost, transform.position) > carvingMoveThreshold)
        {
            lastMoveTime = Time.time;
            lastPost = transform.position;
        }
        if(lastMoveTime + carvingTime < Time.time)
        {
            agent.enabled = false;
            obstacle.enabled = true;
        }
    }

    public void SetDestination(Vector3 destination)
    {
        obstacle.enabled = false;
        lastMoveTime = Time.time;
        lastPost = transform.position;

        StartCoroutine(MoveAgent(destination));
    }

    private IEnumerator MoveAgent(Vector3 destination)
    {
        yield return null;
        agent.enabled = true;
        agent.SetDestination(destination);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
