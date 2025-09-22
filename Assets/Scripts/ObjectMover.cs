using UnityEngine;
using DG.Tweening;

public class ObjectMover : MonoBehaviour
{

    [SerializeField] private Vector3 destinationPos;
    [SerializeField] private float duration;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        //startPos = transform.position;
    }

    public void MoveToDestination()
    {        
        if (audioSource != null)
        {
            audioSource.Play();
        }
        transform.DOLocalMove(destinationPos, duration);
    }

}
