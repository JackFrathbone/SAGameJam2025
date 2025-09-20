using UnityEngine;

public class PlayerDamager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _damage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (collision.collider.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.TakeDamage(_damage);
            }
        }
    }
}
