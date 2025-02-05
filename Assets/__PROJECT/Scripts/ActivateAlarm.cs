using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAlarm : MonoBehaviour
{

    // Liste des spots de lumière à modifier
    public GameObject[] roomSpotlights;

    // Liste des lumières à modifier
    private GameObject[] roomLights;

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

        // Initialiser le tableau de la même taille que roomLights
        roomLights = new GameObject[roomSpotlights.Length];

        // Récupérer tous les enfants (un par parent)
        for (int i = 0; i < roomSpotlights.Length; i++)
        {
            if (roomSpotlights[i].transform.childCount > 0) // Vérifie si un enfant existe
            {
                roomLights[i] = roomSpotlights[i].transform.GetChild(0).gameObject;
            }
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
        if (isBlinking) { 
            Debug.Log("Alarme activée");
            TurnOffAllLights();
        } else 
        { 
            Debug.Log("Alarme désactivée"); 
        }
    }

    void TurnOffAllLights()
    {
        Light[] allLights = FindObjectsOfType<Light>(); // Trouve toutes les lumières dans la scène

        foreach (Light light in allLights)
        {
            light.enabled = false; // Éteint chaque lumière
        }

        Debug.Log("Toutes les lumières ont été éteintes !");
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
