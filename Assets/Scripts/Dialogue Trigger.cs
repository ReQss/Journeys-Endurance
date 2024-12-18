using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public Dialogue dialogue;
    public string sceneName = null;
    public bool isSpecialAction;
    private Transform player;
    public float detectionRadius = 5f;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        player = GameObject.Find("Player").transform;
    }
    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, sceneName, isSpecialAction);
    }
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= detectionRadius && Input.GetKeyDown(KeyCode.E))
        {
            TriggerDialogue();
        }
    }
}
