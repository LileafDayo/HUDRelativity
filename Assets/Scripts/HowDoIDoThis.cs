using UnityEngine;

public class HowDoIDoThis : MonoBehaviour
{
    private float x;
    private float y;
    private float z;
    public float Speed = 1.0f;
    public float beta = 0.7f;
    public bool isdebug = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time;
        float dx = time * Speed;
        float newX = x + dx;
        float distance = Mathf.Sqrt(newX*newX + y*y + z*z);

        float cos = z/distance;
        float cos2 = (cos + beta)/(1 + beta*cos);
        float sin2 = Mathf.Sqrt(1 - cos2*cos2);

        if (newX < 0) sin2 = -1*sin2;
        float anotherX = distance*sin2;
        float anotherZ = distance*cos2;
        transform.position = new Vector3(anotherX, y, anotherZ);

    }
}
