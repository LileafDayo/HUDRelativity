using UnityEngine;

public class WhatIsThisMesh : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //sample mesh and log vertices
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.mesh;
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                Debug.Log($"Vertives {i}: {vertices[i]}");

            }
            //sample scale
            Vector3 scale = transform.localScale;
            Debug.Log($"Scale: {scale}");
        }
        else
        {
            Debug.LogError("No MeshFilter found on this GameObject!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
