using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private Direction fireDirection = Direction.Right;
    [SerializeField] private float fireInterval = 2f;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float projectileSize = 0.5f;
    [SerializeField] private GameObject projectilePrefab;

    private void Start()
    {
        StartCoroutine(FireRoutine());
    }

    private IEnumerator FireRoutine()
    {
        yield return new WaitForSeconds(fireInterval);
        while (true)
        {
            Fire();
            yield return new WaitForSeconds(fireInterval);
        }
    }

    private void Fire()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Turret: projectilePrefab is not assigned.", this);
            return;
        }

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.direction = fireDirection;
            projectile.speed = projectileSpeed;
            projectile.size = projectileSize;
        }
        else
        {
            Debug.LogWarning("Turret: projectilePrefab is missing a Projectile component.", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EscortTarget escort = other.GetComponent<EscortTarget>();
        if (escort != null)
        {
            escort.Die();
        }
    }
}
