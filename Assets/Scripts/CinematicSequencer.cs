using UnityEngine;

public class CinematicSequencer : MonoBehaviour
{

    [SerializeField]
    private GameObject npc1;

    [SerializeField]
    private GameObject npc2;

    [SerializeField]
    private GameObject[] gameObjects;

    private int currentIndex = 0;

    void Start()
    {
        if (gameObjects == null || gameObjects.Length == 0)
        {
            Debug.LogError("The 'gameObjects' array is empty. Please assign GameObjects in the Inspector.");
            return;
        }

        foreach (GameObject obj in gameObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        if (gameObjects[0] != null)
        {
            gameObjects[0].SetActive(true);
        }
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SwitchToNextObject();
        }

        GameObject[] gameObjects = FindObjectsOfType<GameObject>(true);

        foreach (GameObject gameobject in gameObjects)
        {
            if (gameobject.gameObject.name == "Line1" ||
                gameobject.gameObject.name == "Line3" ||
                gameobject.gameObject.name == "Line5" ||
                gameobject.gameObject.name == "Line7" )
            {
                GameObject myObject = gameobject.gameObject;

                if (myObject.activeInHierarchy)
                {
                    npc1.SetActive(true);
                    npc2.SetActive(false);
                }

            }
        }

        foreach (GameObject gameobject in gameObjects)
        {
            if (gameobject.gameObject.name == "Line2" ||
                gameobject.gameObject.name == "Line4" ||
                gameobject.gameObject.name == "Line6" ||
                gameobject.gameObject.name == "Line8")
            {
                GameObject myObject = gameobject.gameObject;

                if (myObject.activeInHierarchy)
                {
                    npc1.SetActive(false);
                    npc2.SetActive(true);
                }

            }
        }
    }


    private void SwitchToNextObject()
    {
        if (gameObjects[currentIndex] != null)
        {
            gameObjects[currentIndex].SetActive(false);
        }

        currentIndex = (currentIndex + 1) % gameObjects.Length;

        if (gameObjects[currentIndex] != null)
        {
            gameObjects[currentIndex].SetActive(true);
        }
    }
}
