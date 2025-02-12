using UnityEngine;

public class DoorController : MonoBehaviour
{
    // Position de la porte fermée
    public Vector3 closedPosition;
    // Position de la porte ouverte
    public Vector3 openPosition;
    // Durée de l'animation d'ouverture (en secondes)
    public float duration = 1.5f;
    private AudioSource audioSource;
    private bool isOpening = false;
    private float timeElapsed = 0f;

    void Start()
    {
        // Initialiser la position de la porte à la position fermée
        transform.localPosition = closedPosition;
        // Récupérer le composant AudioSource attaché à la porte
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isOpening)
        {
            // Augmenter le temps écoulé
            timeElapsed += Time.deltaTime;
            // Calculer le pourcentage d'animation complété
            float t = timeElapsed / duration;
            // Interpoler la position de la porte entre fermée et ouverte
            transform.localPosition = Vector3.Lerp(closedPosition, openPosition, t);

            // Vérifier si l'animation est terminée
            if (timeElapsed >= duration)
            {
                isOpening = false;
                timeElapsed = 0f;
                gameObject.SetActive(false);
            }
        }
    }

    // Méthode pour ouvrir la porte
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
