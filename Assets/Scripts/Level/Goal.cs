using UnityEngine;

// depends on GameManager (Transition agent)
public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        EscortTarget escort = other.GetComponent<EscortTarget>();
        if (escort != null)
        {
            GameManager.Instance.OnEscortRescued(escort);
        }
    }
}
