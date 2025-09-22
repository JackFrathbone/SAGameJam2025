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
    [SerializeField] public BossImageTextPair[] boss_imageTextPairCombo;

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


  

        if (other.gameObject.CompareTag("BossPrompt"))
        {
            speakerUI.SetActive(true);

            BossImageTextPair boss = boss_imageTextPairCombo[0];

            if (characterImg != null && boss.boss_character != null)
            {
                characterImg.sprite = boss.boss_character;
            }

            if (dialogueTMP != null && !string.IsNullOrEmpty(boss.boss_dialogue))
            {
                dialogueTMP.text = FormatDialogueText(boss.boss_dialogue);
            }


            Destroy(other.gameObject);
            StartCoroutine("DisableUI");

        }

        if (other.gameObject.CompareTag("Tut1"))
        {   
            speakerUI.SetActive(true);

            TutorialImageTextPair tut_currentPair = tut_imageTextPairCombo[0];

            if (characterImg != null && tut_currentPair.tut_character != null)
            {
                characterImg.sprite = tut_currentPair.tut_character;
            }

            if (dialogueTMP != null && !string.IsNullOrEmpty(tut_currentPair.tut_dialogue))
            {
                dialogueTMP.text = FormatDialogueText(tut_currentPair.tut_dialogue);
            }


            StartCoroutine("DisableTutUI");
                
        }

        if (other.gameObject.CompareTag("Tut2"))
        {
            speakerUI.SetActive(true);

            TutorialImageTextPair tut_currentPair = tut_imageTextPairCombo[1];

            if (characterImg != null && tut_currentPair.tut_character != null)
            {
                characterImg.sprite = tut_currentPair.tut_character;
            }

            if (dialogueTMP != null && !string.IsNullOrEmpty(tut_currentPair.tut_dialogue))
            {
                dialogueTMP.text = FormatDialogueText(tut_currentPair.tut_dialogue);
            }

            StartCoroutine("DisableTutUI");

        }

        if (other.gameObject.CompareTag("Tut3"))
        {
            speakerUI.SetActive(true);

            TutorialImageTextPair tut_currentPair = tut_imageTextPairCombo[2];

            if (characterImg != null && tut_currentPair.tut_character != null)
            {
                characterImg.sprite = tut_currentPair.tut_character;
            }

            if (dialogueTMP != null && !string.IsNullOrEmpty(tut_currentPair.tut_dialogue))
            {
                dialogueTMP.text = FormatDialogueText(tut_currentPair.tut_dialogue);
            }

            StartCoroutine("DisableTutUI");

        }

        if (other.gameObject.CompareTag("Tut4"))
        {
            speakerUI.SetActive(true);

            TutorialImageTextPair tut_currentPair = tut_imageTextPairCombo[3];

            if (characterImg != null && tut_currentPair.tut_character != null)
            {
                characterImg.sprite = tut_currentPair.tut_character;
            }

            if (dialogueTMP != null && !string.IsNullOrEmpty(tut_currentPair.tut_dialogue))
            {
                dialogueTMP.text = FormatDialogueText(tut_currentPair.tut_dialogue);
            }

            StartCoroutine("DisableTutUI");

        }

        if (other.gameObject.CompareTag("Tut5"))
        {
            speakerUI.SetActive(true);

            TutorialImageTextPair tut_currentPair = tut_imageTextPairCombo[4];

            if (characterImg != null && tut_currentPair.tut_character != null)
            {
                characterImg.sprite = tut_currentPair.tut_character;
            }

            if (dialogueTMP != null && !string.IsNullOrEmpty(tut_currentPair.tut_dialogue))
            {
                dialogueTMP.text = FormatDialogueText(tut_currentPair.tut_dialogue);
            }

            StartCoroutine("DisableTutUI");

        }

        if (other.gameObject.CompareTag("Tut6"))
        {
            speakerUI.SetActive(true);

            TutorialImageTextPair tut_currentPair = tut_imageTextPairCombo[5];

            if (characterImg != null && tut_currentPair.tut_character != null)
            {
                characterImg.sprite = tut_currentPair.tut_character;
            }

            if (dialogueTMP != null && !string.IsNullOrEmpty(tut_currentPair.tut_dialogue))
            {
                dialogueTMP.text = FormatDialogueText(tut_currentPair.tut_dialogue);
            }

            StartCoroutine("DisableTutUI");

        }

        if (other.gameObject.CompareTag("Tut7"))
        {
            speakerUI.SetActive(true);

            TutorialImageTextPair tut_currentPair = tut_imageTextPairCombo[6];

            if (characterImg != null && tut_currentPair.tut_character != null)
            {
                characterImg.sprite = tut_currentPair.tut_character;
            }

            if (dialogueTMP != null && !string.IsNullOrEmpty(tut_currentPair.tut_dialogue))
            {
                dialogueTMP.text = FormatDialogueText(tut_currentPair.tut_dialogue);
            }

            StartCoroutine("DisableTutUI");

        }

        if (other.gameObject.CompareTag("Tut8"))
        {
            speakerUI.SetActive(true);

            TutorialImageTextPair tut_currentPair = tut_imageTextPairCombo[7];

            if (characterImg != null && tut_currentPair.tut_character != null)
            {
                characterImg.sprite = tut_currentPair.tut_character;
            }

            if (dialogueTMP != null && !string.IsNullOrEmpty(tut_currentPair.tut_dialogue))
            {
                dialogueTMP.text = FormatDialogueText(tut_currentPair.tut_dialogue);
            }

            StartCoroutine("DisableTutUI");

        }

        if (other.gameObject.CompareTag("Tut9"))
        {
            speakerUI.SetActive(true);

            TutorialImageTextPair tut_currentPair = tut_imageTextPairCombo[8];

            if (characterImg != null && tut_currentPair.tut_character != null)
            {
                characterImg.sprite = tut_currentPair.tut_character;
            }

            if (dialogueTMP != null && !string.IsNullOrEmpty(tut_currentPair.tut_dialogue))
            {
                dialogueTMP.text = FormatDialogueText(tut_currentPair.tut_dialogue);
            }

            StartCoroutine("DisableTutUI");

        }

        if (other.gameObject.CompareTag("Tut10"))
        {
            speakerUI.SetActive(true);

            TutorialImageTextPair tut_currentPair = tut_imageTextPairCombo[9];

            if (characterImg != null && tut_currentPair.tut_character != null)
            {
                characterImg.sprite = tut_currentPair.tut_character;
            }

            if (dialogueTMP != null && !string.IsNullOrEmpty(tut_currentPair.tut_dialogue))
            {
                dialogueTMP.text = FormatDialogueText(tut_currentPair.tut_dialogue);
            }

            StartCoroutine("DisableTutUI");

        }

        if (other.gameObject.CompareTag("Tut11"))
        {
            speakerUI.SetActive(true);

            TutorialImageTextPair tut_currentPair = tut_imageTextPairCombo[10];

            if (characterImg != null && tut_currentPair.tut_character != null)
            {
                characterImg.sprite = tut_currentPair.tut_character;
            }

            if (dialogueTMP != null && !string.IsNullOrEmpty(tut_currentPair.tut_dialogue))
            {
                dialogueTMP.text = FormatDialogueText(tut_currentPair.tut_dialogue);
            }

            StartCoroutine("DisableTutUI");

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

[System.Serializable]
public class BossImageTextPair
{
    public Sprite boss_character;
    public string boss_dialogue;
}