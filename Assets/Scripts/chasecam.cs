using UnityEngine;

public class chasecam : MonoBehaviour
{
    
    // camera script

    // defines all the objects we need to steal values from and all the variables we will need
    public GameObject player; // player to tape the camera to
    public GameObject slider; // camera slider
    private float senslocal = 2.0f; // receives the number from the camera slider
    public float sens = 2.0f; // unused
    private float x; // x
    private float y; // y
    private Vector3 rotateValue; // the magic that moves the cam

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // OH MY GOD? NEW VECTOR3? LIKE, FROM THE HIT SOFTWARE UNITY???
        // senslocal = slider.GetComponent<sensslider>().sensitivity; // this grabs the sensitivity value of the camera slider
        y = Input.GetAxis("Mouse X")*senslocal; // legacy input system. grabs mouse movement as a quantity and multiply by sens
        x = Input.GetAxis("Mouse Y")*senslocal; // legacy input system. grabs mouse movement as a quantity and multiply by sens
        rotateValue = new Vector3(x,y*-1,0); // target coordinates for the camera
        transform.eulerAngles = transform.eulerAngles - rotateValue; // angular transform the camera's angle until it reaches the target
    }
}
