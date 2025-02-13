using UnityEngine;
using UnityEngine.EventSystems; // Nécessaire pour OnPointerDown / OnPointerUp
public class TurnWheelGradual : MonoBehaviour
{
    public ParticleSystem gasParticleSystem;  // Référence au gaz
    public float rotationSpeed = 50f;         // Vitesse de rotation en degrés/seconde
    public float maxRotation = 90f;           // Angle max du volant
    public float minRotation = 0f;            // Angle min du volant
    public float minEmissionRate = 0f;        // Débit min des particules
    public float maxEmissionRate = 50f;       // Débit max des particules
    public GameObject old_image;
    public GameObject new_image;
    private bool isTurning = false;           // Vérifie si on maintient l’interaction
    private float currentRotation = 0f;       // Rotation actuelle du volant
    public GameManager gameManager;
    public TMPro.TextMeshProUGUI fixGazLeak;
    public static int playerScore = 0; // Variable globale statique

    void Start()
    {
        currentRotation = transform.localEulerAngles.x;
    }

    void Update()
    {
        if (isTurning)
        {
            RotateWheel(); // Continue la rotation tant que isTurning est vrai
        }
    }

    // Quand on appuie sur le volant (OnPointerDown), commence la rotation
    public void OnPointerDown()
    {
        isTurning = true;
    }
    // Quand on relâche le volant (OnPointerUp), arrête la rotation
    public void OnPointerUp()
    {
        isTurning = false;
    }
    private void RotateWheel()
    {
        float rotationStep = rotationSpeed * Time.deltaTime;
        float newRotation = Mathf.Clamp(currentRotation + rotationStep, minRotation, maxRotation);
        if (newRotation != currentRotation)
        {
            currentRotation = newRotation;
            transform.localEulerAngles = new Vector3(currentRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);
            AdjustGasEmission();
        }
    }

    // Ajuste l'intensité des particules en fonction de la rotation
    void AdjustGasEmission()
    {
        if (gasParticleSystem != null)
        {
            var emission = gasParticleSystem.emission;
            float normalizedRotation = (currentRotation - minRotation) / (maxRotation - minRotation);
            float newEmissionRate = Mathf.Lerp(maxEmissionRate, minEmissionRate, normalizedRotation);
            emission.rateOverTime = newEmissionRate;

            if (newEmissionRate <= 0.1f)
            {
                gasParticleSystem.Stop();
                playerScore += 1;
                fixGazLeak.text = $"2. Fix gaz leak ({playerScore}/2)";
                old_image.SetActive(false);
                new_image.SetActive(true);
                if (playerScore  == 2)
                {
                    gameManager.Victory();
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