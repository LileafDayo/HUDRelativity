using System;
using UnityEngine;

public class MoveBox : MonoBehaviour
{
    private float beta1; // A variable to store the relative speed from beta controller
    private GameObject objectBeta;
    private GameObject objectEnd;
    public float relativeSpeed;
    private float startX;
    private float endX;
    public float speed = 5f;
    private float x_0;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        x_0 = transform.position.x;
        startX = x_0;

        objectEnd = GameObject.FindGameObjectWithTag("Endline");
        endX = -1*objectEnd.transform.position.x;
    }

    void Update()
    {
        objectBeta = GameObject.FindGameObjectWithTag("BetaText");
        beta1 = objectBeta.GetComponent<BetaText>().beta;  

        x_0 = transform.position.x;

        relativeSpeed = beta1;

        if (x_0 < endX/2)
        {
            // Calculate the new x position based on time and speed
            float time = Time.time; // Time since the start of the game
            float dx = relativeSpeed * time * speed; // Change in x position
            float newX = x_0 + dx; // New x position
            transform.position = new Vector3(newX, transform.position.y, transform.position.z); // Update the object's position
        }
        else
        {
            transform.position = new Vector3(-1*endX/2, transform.position.y, transform.position.z);
        }
    }
}
