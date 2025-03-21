using UnityEngine;
using UnityEngine.UI;

public class BetaText : MonoBehaviour
{
    // Written by Uzi: handles beta display on HUD
    [SerializeField] private Text _title; // make the text legally exist
    public float beta = 0.5f; // this will have its own beta for simplicity's sake
    private float verticalInput; // quantify up
    private float inputScale = 0.05f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       verticalInput = Input.GetAxis("Vertical"); // arrowkeys pressed?

        beta += verticalInput*inputScale; // scale the true/false up input
        if (beta > 0.9999f) beta = 0.9999f; // limit beta to below c
        if (beta < 0f) beta = 0.01f; // invent perpetual motion
        _title.text = "RELATIVE VELOCITY is " + beta.ToString("#.0000") + "\nRELATIVISTIC EFFECT STRENGTH is " + "x"; // update our textbox


        
    }
}
