using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAlarm : MonoBehaviour
{
    // Liste des lumières à modifier
    public GameObject[] roomLights;

    // Liste des spots de lumière à modifier
    public GameObject[] roomSpotlights;

    // Durée totale du cycle (secondes)
    public float duration = 1f;

    // Durée de l'effet (en secondes)
    public float effectTime = 20f;

    // Temps écoulé pour l'effet complet
    private float effectElapsedTime = 0f;

    private bool isBlinking = false;

    // Référence à l'AudioSource de l'alarme
    public AudioSource alarmAudioSource;

    // Référence au clip audio de l'alarme
    public AudioClip alarmClip;

    // Indicateur pour éviter que l'alarme ne redémarre plusieurs fois
    private bool alarmPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        if (alarmAudioSource != null && alarmClip != null)
        {
            alarmAudioSource.clip = alarmClip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isBlinking)
        {
            // Si l'alarme n'a pas encore été jouée, on la joue en boucle
            if (!alarmPlayed)
            {
                PlayAlarm();
            }

            // Interpolation de couleur pour les lumières
            float t = Mathf.PingPong(Time.time, 1f);

            foreach (GameObject light in roomLights)
            {
                light.GetComponent<Light>().color = Color.Lerp(Color.red, Color.white, t);
            }

            foreach (GameObject spotLight in roomSpotlights)
            {
                // Référence à l'objet
                Renderer renderer = spotLight.GetComponent<Renderer>();

                // Assurez-vous que le matériau utilise un shader supportant l'émission
                Material material = renderer.materials[1];

                // Assurez-vous que l'émission est activée
                material.EnableKeyword("_EMISSION");

                // Modifiez la couleur d'émission avec une interpolation
                material.SetColor("_EmissionColor", Color.Lerp(Color.red, Color.white, t));
            }

            effectElapsedTime += Time.deltaTime;
        }
        else
        {
            // Réinitialiser la couleur des lumières lorsque le blinking est arrêté
            foreach (GameObject light in roomLights)
            {
                light.GetComponent<Light>().color = Color.white;
            }

            // Arrêter l'alarme quand on arrête l'effet
            StopAlarm();
        }
    }

    public void ToggleBlinking()
    {
        isBlinking = !isBlinking;
    }

    // Fonction pour jouer l'alarme en boucle
    void PlayAlarm()
    {
        if (alarmAudioSource != null && alarmClip != null)
        {
            alarmAudioSource.loop = true;  // Mettre l'alarme en boucle
            alarmAudioSource.Play();
            alarmPlayed = true; // Empêcher de jouer l'alarme plusieurs fois
        }
    }

    // Fonction pour arrêter l'alarme
    void StopAlarm()
    {
        if (alarmAudioSource != null)
        {
            alarmAudioSource.Stop();  // Arrêter l'alarme
            alarmPlayed = false; // Permettre de jouer l'alarme à nouveau si isBlinking devient true
        }
    }
}
