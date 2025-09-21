using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialogueSystem : MonoBehaviour
{
    [SerializeField] public ImageTextPair[] imageTextPairCombo;
    [SerializeField] public TutorialImageTextPair[] tut_imageTextPairCombo;

    [SerializeField] private float seconds;
    [SerializeField] private GameObject speakerUI;
    [SerializeField] private TextMeshProUGUI dialogueTMP;
    [SerializeField] private Image characterImg;

    private int currentDialogueIndex = 0;
    private void Start()
    {
        speakerUI.SetActive(false);
        characterImg.sprite = imageTextPairCombo[0].character;
        dialogueTMP.text = imageTextPairCombo[0].dialogue;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SpeakerPrompt"))
        {
            speakerUI.SetActive(true);
            int randomIndex = Random.Range(0, imageTextPairCombo.Length);
            ImageTextPair randomPair = imageTextPairCombo[randomIndex];

            if (characterImg != null && randomPair.character != null)
            {
                characterImg.sprite = randomPair.character;
            }

            if (dialogueTMP != null && !string.IsNullOrEmpty(randomPair.dialogue))
            {
                dialogueTMP.text = randomPair.dialogue;
            }

            Destroy(other.gameObject);
            StartCoroutine("DisableUI");

        }

        if (other.gameObject.CompareTag("TutorialPrompt"))
        {
                if (currentDialogueIndex < tut_imageTextPairCombo.Length)
                {
                    speakerUI.SetActive(true);

                    TutorialImageTextPair tut_currentPair = tut_imageTextPairCombo[currentDialogueIndex];

                    if (characterImg != null && tut_currentPair.tut_character != null)
                    {
                        characterImg.sprite = tut_currentPair.tut_character;
                    }

                    if (dialogueTMP != null && !string.IsNullOrEmpty(tut_currentPair.tut_dialogue))
                    {
                        dialogueTMP.text = tut_currentPair.tut_dialogue;
                    }

                    currentDialogueIndex++;

                    Destroy(other.gameObject);
                    StartCoroutine("DisableUI");
                }
            }
        }


    IEnumerator DisableUI()
    {
        yield return new WaitForSeconds(seconds);
        speakerUI.SetActive(false);
    }

}

[System.Serializable]
public class ImageTextPair
{
    public Sprite character;
    public string dialogue;
}


[System.Serializable]
public class TutorialImageTextPair
{
    public Sprite tut_character;
    public string tut_dialogue;
}