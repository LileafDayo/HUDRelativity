using UnityEditor;
using UnityEngine;

public class AberrationAngle : MonoBehaviour
{
    private Mesh mesh; // The mesh of the object this script is attached to
    private Vector3[] originalVertices; // Stores the original vertices of the mesh
    private Vector3[] modifiedVertices; // Stores the modified vertices after calculations
    private Vector3[] baseVertices; // Stores the vertices after applying scale and rotation
    public float relativeSpeed = 0.5f; // Speed relative to light
    private float beta1; // A variable to store the relative speed from beta controller
    private GameObject objectBeta; // Reference to the GameObject with the relative
    private float x_0; // Initial x position of the object
    private float y_0; // Initial y position of the object
    private float z_0; // Initial z position of the object

    // Start is called before the first frame update
    void Start()
    {
        // Try to get the MeshFilter component attached to this GameObject
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            // Assign the mesh from the MeshFilter
            mesh = meshFilter.mesh; // Use mesh instead of sharedMesh to avoid affecting all instances of this mesh
        }
        else
        {
            // Log an error if no MeshFilter is found
            Debug.LogError("No MeshFilter found on this GameObject!");
            return;
        }

        // Store the original vertices of the mesh
        originalVertices = mesh.vertices;
        // Initialize arrays for modified and base vertices
        modifiedVertices = new Vector3[originalVertices.Length];
        baseVertices = new Vector3[originalVertices.Length];

        // Copy and process each vertex
        for (int i = 0; i < originalVertices.Length; i++)
        {
            // Copy the original vertex to the modified array
            modifiedVertices[i] = originalVertices[i];

            // Apply the object's local scale to the vertex
            Vector3 scaledVertex = new Vector3(
                originalVertices[i].x * transform.localScale.x,
                originalVertices[i].y * transform.localScale.y,
                originalVertices[i].z * transform.localScale.z
            );
            // Debug.Log($"Before: {scaledVertex}"); // Debug log to show the vertex before rotation

            // Apply the object's rotation to the scaled vertex
            baseVertices[i] = transform.rotation * scaledVertex;
            // Debug.Log($"After: {baseVertices[i]}"); // Debug log to show the vertex after rotation
        }

        // Find the GameObject tagged as "BetaText" and get its beta value
        objectBeta = GameObject.FindGameObjectWithTag("BetaText");
        beta1 = objectBeta.GetComponent<BetaText>().beta;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the reference to the "BetaText" GameObject and its beta value
        objectBeta = GameObject.FindGameObjectWithTag("BetaText");
        beta1 = objectBeta.GetComponent<BetaText>().beta;

        // Store the current position of the object
        x_0 = transform.position.x;
        y_0 = transform.position.y;
        z_0 = transform.position.z;

        // If the mesh is null, exit the function
        if (mesh == null) return;

        // Update the relative speed with the beta value
        relativeSpeed = beta1;

        // Update each vertex of the mesh
        for (int i = 0; i < modifiedVertices.Length; i++)
        {
            // Calculate the global position of the vertex
            float x = x_0 + baseVertices[i].x;
            float y = y_0 + baseVertices[i].y;
            float z = z_0 + baseVertices[i].z;

            // Perform calculations for vertex transformation
            float dist = Mathf.Sqrt(x * x + y * y + z * z); // Distance from the origin
            float dzy = Mathf.Sqrt(y * y + z * z); // Distance in the yz plane

            // Perform transformations based on equations of aberration
            float cosTheta = (-1) * x / dist;
            float cosThetaModified = (cosTheta + relativeSpeed) / (1 + relativeSpeed * cosTheta);
            float sinThetaModified = Mathf.Sqrt(1 - cosThetaModified * cosThetaModified);

            // sin and cos of phi(azmuthal angle)
            float sinPhi = y / dzy;
            float cosPhi = z / dzy;

            // Calculate the rotated vertex position
            Vector3 rotatedVertex = new Vector3(
                -1 * dist * cosThetaModified - x_0,
                dist * sinThetaModified * sinPhi - y_0,
                dist * sinThetaModified * cosPhi - z_0
            );

            // Reverse the object's rotation to get the local vertex position
            Vector3 localVertex = Quaternion.Inverse(transform.rotation) * rotatedVertex;

            // Scale the vertex back to its original size
            modifiedVertices[i].x = localVertex.x / transform.localScale.x;
            modifiedVertices[i].y = localVertex.y / transform.localScale.y;
            modifiedVertices[i].z = localVertex.z / transform.localScale.z;
        }

        // Apply the modified vertices back to the mesh
        mesh.vertices = modifiedVertices;

        // Recalculate normals and bounds to ensure proper rendering
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
