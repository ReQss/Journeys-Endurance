using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{
    public Animator animator;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image image;
    private Queue<string> sentences;
    [SerializeField]
    public GameObject[] uiToDisable;
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        GameManager.Instance.isPlayerInteracting = true;
        Cursor.lockState = CursorLockMode.None;
        DisableUIElements();
        animator.SetBool("IsOpen", true);
        nameText.text = dialogue.name;
        image.sprite = dialogue.npcImage;
        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }
    public void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        EnableUIElements();
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.isPlayerInteracting = false;

    }
    public void DisableUIElements()
    {
        foreach (GameObject gameObject in uiToDisable)
        {
            gameObject.SetActive(false);
        }
    }
    public void EnableUIElements()
    {
        foreach (GameObject gameObject in uiToDisable)
        {
            gameObject.SetActive(true);
        }
    }
}
