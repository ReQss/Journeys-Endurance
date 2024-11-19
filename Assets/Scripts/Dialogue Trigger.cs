using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public Dialogue dialogue;
    public Transform player;
    public float detectionRadius = 5f;
    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
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
