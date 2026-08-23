using UnityEngine;

public class Boundary : MonoBehaviour
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
