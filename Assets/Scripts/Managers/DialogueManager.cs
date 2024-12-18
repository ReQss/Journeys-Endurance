using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{
    public AudioSource dialogueSound;
    public Animator animator;
    public Animator choiceAnimator;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image image;
    private Queue<string> sentences;
    [SerializeField]
    public GameObject[] uiToDisable;
    public bool specialAction = false;
    public string sceneName = null;
    public bool haveChoice = false;
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue, string sceneName, bool isSpecialAction)
    {
        if (sceneName != null && isSpecialAction)
        {
            this.sceneName = sceneName;
            specialAction = isSpecialAction;
        }

        GameManager.Instance.isPlayerInteracting = true;
        Cursor.lockState = CursorLockMode.None;
        DisableUIElements();
        animator.SetBool("IsOpen", true);
        nameText.text = dialogue.name;
        if (dialogue.npcImage != null)
            image.sprite = dialogue.npcImage;
        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
    public void StartChoice()
    {
        specialAction = false;
        GameManager.Instance.isPlayerInteracting = true;
        Cursor.lockState = CursorLockMode.None;
        DisableUIElements();
        choiceAnimator.SetBool("IsOpen", true);
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
            dialogueSound.enabled = true;
            dialogueText.text += letter;
            yield return null;
        }
        dialogueSound.enabled = false;
    }
    public void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        EnableUIElements();
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.isPlayerInteracting = false;
        if (specialAction)
        {
            StartChoice();
        }
        // if (specialAction) SceneManager.LoadScene(sceneName);

    }
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>

    public void LoadScene()
    {
        InventoryManager.Instance.isInventoryLoaded = false;
        StartCoroutine(LoadSceneWithInventory());
        // SceneManager.LoadScene(sceneName);
        // InventoryManager.Instance.LoadGameItemsPrefabs();
        // InventoryManager.Instance.LoadItems();
    }
    public IEnumerator LoadSceneWithInventory()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2);

        InventoryManager.Instance.LoadGameItemsPrefabs();
        InventoryManager.Instance.LoadItems();
    }
    public void EndChoice()
    {
        choiceAnimator.SetBool("IsOpen", false);
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
