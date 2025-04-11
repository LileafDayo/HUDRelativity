using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Controller : MonoBehaviour {
    [SerializeField] private Text velocityDisplay;
    public float beta = 0f;
    private DopplerEffectVolume _volume;
    private GameObject objectBeta; // Reference to the GameObject with the relative
    private float beta1; // A variable to store the relative speed from beta controller


    void Start() {
        _volume = VolumeManager.instance.stack.GetComponent<DopplerEffectVolume>();
    }

    void Update() {
        
        
        objectBeta = GameObject.FindGameObjectWithTag("BetaText");
        beta1 = objectBeta.GetComponent<BetaText>().beta;
        beta = beta1;

        if (_volume != null) {
            _volume.beta.value = beta;
            _volume.intensity.value = Mathf.Clamp01(Mathf.Abs(beta) * 2f);
        }
    }
}