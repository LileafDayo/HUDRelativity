using UnityEngine;
using UnityEngine.UI;

public class camtext : MonoBehaviour
{
    // Written by Uzi: handles cam display on HUD
    [SerializeField] private Text _title; // make the text legally exist
    private float sens = 2.0f; // this will have its own sens for now (TODO)
    private float horizontalInput; // quantify up
    private float inputScale = 0.05f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // arrowkeys pressed?

        sens += horizontalInput*inputScale; // scale the true/false up input
        if (sens < 0.1f) sens = 0.1f; // don't let the camera sensitivity go negative, it's confusing
        _title.text = "Camera sensitivity: " + sens.ToString("#.0000"); // update our textbox


        
    }
}
