using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCollider : MonoBehaviour
{
    GameObject mesh;
    // Start is called before the first frame update
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        mesh = GameObject.Find("Mesh");

        mesh.AddComponent<MeshCollider>();
    }
}
