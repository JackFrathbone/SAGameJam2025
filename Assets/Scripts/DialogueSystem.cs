using Mono.Cecil.Cil;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialogueSystem : MonoBehaviour
{
    [SerializeField] public ImageTextPair[] imageTextPairCombo;

    [SerializeField] private float seconds;
    [SerializeField] private GameObject speakerUI;
    [SerializeField] private TextMeshProUGUI dialogueTMP;
    [SerializeField] private Image characterImg;

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
            StartCoroutine("PrintAfterDelay");

        }
    }


    IEnumerator PrintAfterDelay()
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