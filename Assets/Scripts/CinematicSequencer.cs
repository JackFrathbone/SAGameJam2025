using UnityEngine;

public class CinematicSequencer : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject npc1;

    [SerializeField]
    private GameObject npc2;

    [SerializeField]
    private GameObject[] gameObjects;

    private int currentIndex = 0;

    [SerializeField]
    private GameObject OldMusic;

    [SerializeField]
    private GameObject NewMusic;

    [SerializeField]
    private GameObject RecordScratch;


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

        OldMusic.SetActive(true);
        NewMusic.SetActive(false);
        RecordScratch.SetActive(false);
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
                gameobject.gameObject.name == "Line4" ||
                gameobject.gameObject.name == "Line6" ||
                gameobject.gameObject.name == "Line7" ||
                gameobject.gameObject.name == "Line9" ||
                gameobject.gameObject.name == "Line13")
            {
                GameObject myObject = gameobject.gameObject;

                if (myObject.activeInHierarchy)
                {
                    npc1.SetActive(true);
                    npc2.SetActive(false);
                    player.SetActive(false);
                }

            }
        }

        foreach (GameObject gameobject in gameObjects)
        {
            if (gameobject.gameObject.name == "Line2" ||
                gameobject.gameObject.name == "Line5" ||
                gameobject.gameObject.name == "Line8" ||
                gameobject.gameObject.name == "Line12")
            {
                GameObject myObject = gameobject.gameObject;

                if (myObject.activeInHierarchy)
                {
                    npc1.SetActive(false);
                    npc2.SetActive(true);
                    player.SetActive(false);
                }

            }
        }

        foreach (GameObject gameobject in gameObjects)
        {
            if (gameobject.gameObject.name == "Line10" ||
                gameobject.gameObject.name == "Line11" ||
                gameobject.gameObject.name == "Line14" ||
                gameobject.gameObject.name == "Line15")
            {
                GameObject myObject = gameobject.gameObject;

                if (myObject.activeInHierarchy)
                {
                    npc1.SetActive(false);
                    npc2.SetActive(false);
                    player.SetActive(true);
                }

            }

            if (gameobject.gameObject.name == "Line10")
            {
                GameObject myObject = gameobject.gameObject;

                if (myObject.activeInHierarchy)
                {
                    OldMusic.SetActive(false);
                    NewMusic.SetActive(false);
                    RecordScratch.SetActive(true);
                }
            }

            if (gameobject.gameObject.name == "Line11" || 
                gameobject.gameObject.name == "Line12" ||
                gameobject.gameObject.name == "Line13" ||
                gameobject.gameObject.name == "Line14" ||
                gameobject.gameObject.name == "Line15")
            {
                GameObject myObject = gameobject.gameObject;

                if (myObject.activeInHierarchy)
                {
                    OldMusic.SetActive(false);
                    NewMusic.SetActive(true);
                    RecordScratch.SetActive(false);
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


        if(currentIndex == 14)
        {
            Debug.Log("GAME START");
        }
    }
}
