using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAlarm : MonoBehaviour
{

    // Liste des spots de lumi�re � modifier
    public GameObject[] roomSpotlights;

    // Liste des lumi�res � modifier
    private GameObject[] roomLights;

    // Dur�e totale du cycle (secondes)
    public float duration = 1f;

    // Dur�e de l'effet (en secondes)
    public float effectTime = 20f;

    // Temps �coul� pour l'effet complet
    private float effectElapsedTime = 0f;

    private bool isBlinking = false;

    // R�f�rence � l'AudioSource de l'alarme
    public AudioSource alarmAudioSource;

    // R�f�rence au clip audio de l'alarme
    public AudioClip alarmClip;

    // Indicateur pour �viter que l'alarme ne red�marre plusieurs fois
    private bool alarmPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        if (alarmAudioSource != null && alarmClip != null)
        {
            alarmAudioSource.clip = alarmClip;
        }

        // Initialiser le tableau de la m�me taille que roomLights
        roomLights = new GameObject[roomSpotlights.Length];

        // R�cup�rer tous les enfants (un par parent)
        for (int i = 0; i < roomSpotlights.Length; i++)
        {
            if (roomSpotlights[i].transform.childCount > 0) // V�rifie si un enfant existe
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
            // Si l'alarme n'a pas encore �t� jou�e, on la joue en boucle
            if (!alarmPlayed)
            {
                PlayAlarm();
            }

            // Interpolation de couleur pour les lumi�res
            float t = Mathf.PingPong(Time.time, 1f);

            foreach (GameObject light in roomLights)
            {
                light.GetComponent<Light>().color = Color.Lerp(Color.red, Color.white, t);
            }

            foreach (GameObject spotLight in roomSpotlights)
            {
                // R�f�rence � l'objet
                Renderer renderer = spotLight.GetComponent<Renderer>();

                // Assurez-vous que le mat�riau utilise un shader supportant l'�mission
                Material material = renderer.materials[1];

                // Assurez-vous que l'�mission est activ�e
                material.EnableKeyword("_EMISSION");

                // Modifiez la couleur d'�mission avec une interpolation
                material.SetColor("_EmissionColor", Color.Lerp(Color.red, Color.white, t));
            }

            effectElapsedTime += Time.deltaTime;
        }
        else
        {
            // R�initialiser la couleur des lumi�res lorsque le blinking est arr�t�
            foreach (GameObject light in roomLights)
            {
                light.GetComponent<Light>().color = Color.white;
            }

            // Arr�ter l'alarme quand on arr�te l'effet
            StopAlarm();
        }
    }

    public void ToggleBlinking()
    {
        isBlinking = !isBlinking;
        if (isBlinking) { 
            Debug.Log("Alarme activ�e");
            TurnOffAllLights();
        } else 
        { 
            Debug.Log("Alarme d�sactiv�e"); 
        }
    }

    void TurnOffAllLights()
    {
        Light[] allLights = FindObjectsOfType<Light>(); // Trouve toutes les lumi�res dans la sc�ne

        foreach (Light light in allLights)
        {
            light.enabled = false; // �teint chaque lumi�re
        }

        Debug.Log("Toutes les lumi�res ont �t� �teintes !");
    }

    // Fonction pour jouer l'alarme en boucle
    void PlayAlarm()
    {
        if (alarmAudioSource != null && alarmClip != null)
        {
            alarmAudioSource.loop = true;  // Mettre l'alarme en boucle
            alarmAudioSource.Play();
            alarmPlayed = true; // Emp�cher de jouer l'alarme plusieurs fois
        }
    }

    // Fonction pour arr�ter l'alarme
    void StopAlarm()
    {
        if (alarmAudioSource != null)
        {
            alarmAudioSource.Stop();  // Arr�ter l'alarme
            alarmPlayed = false; // Permettre de jouer l'alarme � nouveau si isBlinking devient true
        }
    }
}
