using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        EscortTarget escort = other.GetComponent<EscortTarget>();
        if (escort != null)
        {
            escort.Die();
        }
    }
}
