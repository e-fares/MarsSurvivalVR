using UnityEngine;

public class DoorController : MonoBehaviour
{
    // Position de la porte ferm�e
    public Vector3 closedPosition;
    // Position de la porte ouverte
    public Vector3 openPosition;
    // Dur�e de l'animation d'ouverture (en secondes)
    public float duration = 1.5f;
    private AudioSource audioSource;
    private bool isOpening = false;
    private float timeElapsed = 0f;

    void Start()
    {
        // Initialiser la position de la porte � la position ferm�e
        transform.localPosition = closedPosition;
        // R�cup�rer le composant AudioSource attach� � la porte
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isOpening)
        {
            // Augmenter le temps �coul�
            timeElapsed += Time.deltaTime;
            // Calculer le pourcentage d'animation compl�t�
            float t = timeElapsed / duration;
            // Interpoler la position de la porte entre ferm�e et ouverte
            transform.localPosition = Vector3.Lerp(closedPosition, openPosition, t);

            // V�rifier si l'animation est termin�e
            if (timeElapsed >= duration)
            {
                isOpening = false;
                timeElapsed = 0f;
                gameObject.SetActive(false);
            }
        }
    }

    // M�thode pour ouvrir la porte
    public void OpenDoor()
    {
        if (!isOpening)
        {
            isOpening = true;
            // Jouer le son d'ouverture de la porte
            audioSource.Play();
        }

    }
}
