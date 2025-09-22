using UnityEngine;
using UnityEngine.Events;

public class BasicTrigger : MonoBehaviour
{
    public UnityEvent onTriggerEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerEvent.Invoke();
            Destroy(gameObject);
        }
    }
}
