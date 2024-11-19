using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Dialogue
{
    // Start is called before the first frame update

    public string name;
    // [SerializeField]
    public Sprite npcImage;
    [TextArea(1, 10)]
    public string[] sentences;

}
