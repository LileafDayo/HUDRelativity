using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveBox : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] modifiedVertices;
    private Vector3[] baseVertices;
    public float relativeSpeed;
    private float startX;
    public float speed = 1f;
    public bool isdebug = false;

    private float x_0;
    private float y_0;
    private float z_0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float endX = -1*startX;

        // Get the Mesh component
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            mesh = meshFilter.mesh; // Use mesh instead of sharedMesh to avoid modifying all instances
        }
        else
        {
            Debug.LogError("No MeshFilter found on this GameObject!");
            return;
        }

        // Store original vertices
        originalVertices = mesh.vertices;
        modifiedVertices = new Vector3[originalVertices.Length];
        baseVertices = new Vector3[originalVertices.Length];

        // Copy vertices for modification
        for (int i = 0; i < originalVertices.Length; i++)
        {
            modifiedVertices[i] = originalVertices[i];
            baseVertices[i].x = originalVertices[i].x*transform.localScale.x;
            baseVertices[i].y = originalVertices[i].y*transform.localScale.y;
            baseVertices[i].z = originalVertices[i].z*transform.localScale.z;
        }

    }

    void Update()
    {
        x_0 = transform.position.x;
        y_0 = transform.position.y;
        z_0 = transform.position.z;

        if (mesh == null) return;

        float time = Time.time;
        float dx = relativeSpeed * time * speed;

        float newX = x_0 + dx;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // if (isdebug == true) Debug.Log($"position: {transform.position.x}");
        // if (isdebug == true) Debug.Log($"Vertex {i} distance: {dist}");

        for (int i = 0; i < modifiedVertices.Length; i++)
        {
            float x = newX + baseVertices[i].x;
            float y = y_0 + baseVertices[i].y;
            float z = z_0 + baseVertices[i].z;

            float dist = Mathf.Sqrt(x*x + y*y + z*z);

            // if (isdebug == true & i==1)
            // Debug.Log($"Vertex {i} distance: {dist}");

            float ca = z / dist;
            float caRel = (ca + relativeSpeed) / (1 + relativeSpeed * ca);
            float saRel = Mathf.Sqrt(1 - caRel * caRel);

            
            // if (i == 1) Debug.Log($"baseVertices {i}: {baseVertices[i]}");
            // if (i == 1) Debug.Log($"distance {i}: {dist}");
            

            if (x < 0 ) saRel = -1*saRel;
            // if (i == 1) Debug.Log($"x {i}: {dist*saRel}");
            // if (i == 1) Debug.Log($"distance {i}: {dist}");
            // if (i == 1) Debug.Log($"sin {i}: {saRel}");
            // if (i == 1 & isdebug == true) Debug.Log($"sin {i}: {saRel}");

            if (saRel == 0) saRel = 0.0001f;

            modifiedVertices[i].x = (dist*saRel - x_0)/transform.localScale.x;
            modifiedVertices[i].y = originalVertices[i].y;
            modifiedVertices[i].z = (caRel * dist - z_0)/transform.localScale.z;
           
            // Debug.Log($"distance {i}: {dist}");
            // if (isdebug == true & i == 1 & dist*saRel > -1 & dist*saRel < 1) Debug.Log($"Vertex {i}: {dist*saRel}");
        }

        // Apply changes to the mesh

        mesh.vertices = modifiedVertices;
        // mesh.vertices = baseVertices;
        // mesh.vertices = originalVertices;

        mesh.RecalculateNormals(); // Important to keep shading correct
        mesh.RecalculateBounds();  // Update the bounding box
    }
}
