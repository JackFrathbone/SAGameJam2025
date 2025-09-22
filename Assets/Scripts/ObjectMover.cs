using UnityEngine;
using DG.Tweening;

public class ObjectMover : MonoBehaviour
{

    [SerializeField] private Vector3 destinationPos;
    [SerializeField] private float duration;

    private void Start()
    {
        //startPos = transform.position;
    }

    public void MoveToDestination()
    {        
        transform.DOLocalMove(destinationPos, duration);
    }

}
