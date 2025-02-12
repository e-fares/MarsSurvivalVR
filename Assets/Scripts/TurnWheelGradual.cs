using UnityEngine;
using UnityEngine.UI; // For UI elements

public class TurnWheelGradual : MonoBehaviour
{
    public ParticleSystem gasParticleSystem;  // Reference to gas particle system
    public float maxRotation = 90f;           // Maximum wheel rotation
    public float minRotation = 0f;            // Minimum wheel rotation
    public float minEmissionRate = 0f;        // Minimum particle emission rate
    public float maxEmissionRate = 50f;       // Maximum particle emission rate

    private float currentRotation = 0f;       // Current wheel rotation
    private int clickCount = 0;               // Tracks button clicks (max 3)

    public TMPro.TextMeshProUGUI fixGazLeak;
    public static int playerScore = 0;

    void Start()
    {
        currentRotation = transform.localEulerAngles.x;

    }

    public void OnButtonClick()
    {
        if (clickCount < 3) // 3 clicks needed for full rotation
        {
            clickCount++;
            float rotationStep = (maxRotation - minRotation) / 3; // Divide into 3 steps
            currentRotation = Mathf.Clamp(currentRotation + rotationStep, minRotation, maxRotation);
            transform.localEulerAngles = new Vector3(currentRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);
            AdjustGasEmission();
        }
    }

    // Adjusts the intensity of gas emission based on the number of clicks
    void AdjustGasEmission()
    {
        if (gasParticleSystem != null)
        {
            var emission = gasParticleSystem.emission;
            float normalizedRotation = (float)clickCount / 3; // Goes from 0 to 1 over three clicks
            float newEmissionRate = Mathf.Lerp(maxEmissionRate, minEmissionRate, normalizedRotation);
            emission.rateOverTime = newEmissionRate;

            // Stop gas ONLY when rotation is fully completed
            if (clickCount == 3)
            {
                gasParticleSystem.Stop();
                playerScore += 1;
                fixGazLeak.text = $"2. Fix gas leak ({playerScore / 2}/2)";
                if (playerScore / 2 == 2)
                {
                    fixGazLeak.color = ColorUtility.TryParseHtmlString("#4CFFB3", out Color newColor) ? newColor : fixGazLeak.color;
                }
            }
            else if (!gasParticleSystem.isPlaying)
            {
                gasParticleSystem.Play();
            }
        }
    }
}
