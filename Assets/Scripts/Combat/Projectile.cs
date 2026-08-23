using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Direction direction;
    [SerializeField] private float speed;
    [SerializeField] private float size;

    private Vector3 cachedScale;

    private void Start()
    {
        ApplySize();
    }

    private void ApplySize()
    {
        cachedScale = Vector3.one * size;
        transform.localScale = cachedScale;
    }

    private void Update()
    {
        Vector2 step = DirectionUtility.GetVector(direction) * speed * Time.deltaTime;
        transform.position += (Vector3)step;

        if (transform.localScale != cachedScale)
        {
            ApplySize();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EscortTarget escort = other.GetComponent<EscortTarget>();
        if (escort != null)
        {
            escort.Die();
            Destroy(gameObject);
            return;
        }

        Obstacle obstacle = other.GetComponent<Obstacle>();
        Boundary boundary = other.GetComponent<Boundary>();
        if (obstacle != null || boundary != null)
        {
            Destroy(gameObject);
        }
    }
}
