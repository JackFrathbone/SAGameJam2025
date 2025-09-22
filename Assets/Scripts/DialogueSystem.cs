using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] public ImageTextPair[] imageTextPairCombo;
    [SerializeField] public TutorialImageTextPair[] tut_imageTextPairCombo;

    [SerializeField] private float barkseconds;
    [SerializeField] private float tutseconds;
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
                        dialogueTMP.text = FormatDialogueText(tut_currentPair.tut_dialogue);
                    }

                    currentDialogueIndex++;

                    Destroy(other.gameObject);
                    StartCoroutine("DisableTutUI");
                }
            }
        }


    IEnumerator DisableUI()
    {
        yield return new WaitForSeconds(barkseconds);
        speakerUI.SetActive(false);
    }


    IEnumerator DisableTutUI()
    {
        yield return new WaitForSeconds(tutseconds);
        speakerUI.SetActive(false);
    }

    private string FormatDialogueText(string input)
    {
        return Regex.Replace(input, @"\b[A-Z]+\b", "<color=#C0392B>$0</color>");
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