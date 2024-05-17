using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public Shader terrainShader;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public void DrawTexture(Texture2D texture)
    {

        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }
    public void DrawMesh(MeshData meshData, Texture2D texture2D)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        Shader shader = terrainShader;
        Material material = new Material(shader);
        material.mainTexture = texture2D;
        meshRenderer.sharedMaterial = material;
    }

}
