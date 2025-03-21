using UnityEngine;

public class AberrationAngle : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] modifiedVertices;
    private Vector3[] baseVertices;
    public float relativeSpeed = 0.5f;
    private float beta1;
    private GameObject objectBeta;
    public float speed = 5f;
    public bool isdebug = false;

    private float x_0;
    private float y_0;
    private float z_0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

            // Apply local scale and rotation to base vertices
            Vector3 scaledVertex = new Vector3(
                originalVertices[i].x * transform.localScale.x,
                originalVertices[i].y * transform.localScale.y,
                originalVertices[i].z * transform.localScale.z
            );
            Debug.Log($"Before: {scaledVertex}"); // Debug output for local rotation
            baseVertices[i] = transform.rotation * scaledVertex; // Apply rotation
            Debug.Log ($"After: {baseVertices[i]}");
        }

        // beta1 = relativeSpeed;
        objectBeta = GameObject.FindGameObjectWithTag("BetaText");
        beta1 = objectBeta.GetComponent<BetaText>().beta;
    }

    // Update is called once per frame
    void Update()
    {

        objectBeta = GameObject.FindGameObjectWithTag("BetaText");
        beta1 = objectBeta.GetComponent<BetaText>().beta;
        // Debug.Log($"Beta1: {beta1}");

        x_0 = transform.position.x;
        y_0 = transform.position.y;
        z_0 = transform.position.z;

        if (mesh == null) return;

        relativeSpeed = beta1;

        float time = Time.time;
        float dx = relativeSpeed * time * speed;
        

        float newX = x_0 + dx;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        for (int i = 0; i < modifiedVertices.Length; i++)
        {
            float x = newX + baseVertices[i].x;
            float y = y_0 + baseVertices[i].y;
            float z = z_0 + baseVertices[i].z;

            float dist = Mathf.Sqrt(x*x +y*y + z*z);

            // if (isdebug == true & i==1) Debug.Log($"Vertex {i} distance: {dist}");

            float ca = x / Mathf.Sqrt(x*x + z*z);
            // float ca = x / dist;

            float caRel = (ca + relativeSpeed) / (1 + relativeSpeed * ca);
            float saRel = Mathf.Sqrt(1 - caRel * caRel);
           
            // if (i == 1) Debug.Log($"baseVertices {i}: {baseVertices[i]}");
            // if (i == 1) Debug.Log($"distance {i}: {dist}");
            
            if (z < 0 ) saRel = -1*saRel;

            // if (i == 1) Debug.Log($"x {i}: {dist*saRel}");
            // if (i == 1) Debug.Log($"distance {i}: {dist}");
            // if (i == 1) Debug.Log($"sin {i}: {saRel}");
            // if (i == 1 & isdebug == true) Debug.Log($"sin {i}: {saRel}");

            if (caRel == 0) caRel = 0.00001f;
            if (saRel == 0) saRel = 0.00001f;

            // if (z < 0) saRel = -1*saRel;

            //PLEASE keep this commented out code below because this fixed the bug for some reason
            // modifiedVertices[i].x = (caRel * dist - x_0)/transform.localScale.x;
            // modifiedVertices[i].y = originalVertices[i].y;
            // modifiedVertices[i].z = (dist*saRel - z_0)/transform.localScale.z;
           
            // Convert back from rotated space to local space
            Vector3 rotatedVertex = new Vector3(
                caRel * dist - x_0,
                baseVertices[i].y,
                dist * saRel - z_0
                // baseVertices[i].x, baseVertices[i].y, baseVertices[i].z
            );
            Vector3 localVertex = Quaternion.Inverse(transform.rotation) * rotatedVertex; // Reverse rotation

            modifiedVertices[i].x = localVertex.x / transform.localScale.x;
            modifiedVertices[i].y = localVertex.y / transform.localScale.y;
            modifiedVertices[i].z = localVertex.z / transform.localScale.z;
           
            // Debug.Log($"distance {i}: {dist}");
            // if (isdebug == true & i == 1 & dist*saRel > -1 & dist*saRel < 1) Debug.Log($"Vertex {i}: {dist*saRel}");
        }

        mesh.vertices = modifiedVertices;

        mesh.RecalculateNormals(); // Important to keep shading correct
        mesh.RecalculateBounds();  // Update the bounding box
    }
}
