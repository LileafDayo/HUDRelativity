using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class lorentzslider : MonoBehaviour
{
    // slider that changes the lorentz factor
    // TODO: combine both sliders into a singular script for ease of maintenance?
    // TODO: wrt VR should this even be a slider?

    // define all the variables and objects to steal from
    public float beta = 0.0f; // actual beta
    private float targetbeta = 0.0f; // beta we want to reach
    public float gamma = 0.0f; // gamma
    [SerializeField] private Slider _slider; // this basically allows unity to recognise that the slider exists and is a slider
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        _slider.onValueChanged.AddListener(delegate {ValueChangeCheck(); }); // add a process that is alerted whenever the slider's value is changed and sees how to process that
    }

    // Update is called once per frame
    void Update()
    {
        // rapidly but smoothly transitions effective beta to target value in order to prevent unphysical discontinuous acceleration
        if (beta > (targetbeta))
        {
            beta = (beta) - 0.01f;
        }
        if (beta < (targetbeta))
        {
            beta = (beta) + 0.01f;
        }
        // Debug.Log(beta);
        gamma = 1/Mathf.Sqrt(1-beta); // calculates gamma
    }

    public void ValueChangeCheck() // function for actually handling value changes
    {
        targetbeta = (float)(_slider.value); // set target beta to the slider's value
    }
}
