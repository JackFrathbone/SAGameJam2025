using UnityEngine;

public class WebGLSpecificObjectDisabler : MonoBehaviour
{
    private void Start()
    {
#if UNITY_WEBGL
        gameObject.SetActive(false);
#endif
    }
}
