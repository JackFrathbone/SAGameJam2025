using RenderHeads.Services;
using UnityEngine;
using UnityEngine.Events;

public class BasicTrigger : MonoBehaviour
{
    public UnityEvent onTriggerEvent;
    private LazyService<GameManager> _gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerEvent.Invoke();
            Destroy(gameObject);
        }
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _gameManager.Value.PauseGame();
    }
}
