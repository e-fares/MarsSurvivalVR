using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public Transform player;  // Référence au joueur
    public float activationDistance = 3f;  // Distance pour ouvrir la porte
    public float openHeight = 3f;  // Hauteur à laquelle la porte s'ouvre
    public float openSpeed = 2f;  // Vitesse d'ouverture
    public float closeSpeed = 2f; // Vitesse de fermeture
    public float closeDelay = 3f;  // Délai avant de fermer la porte
    public AudioClip openSound;   // Son à jouer lors de l'ouverture
    public AudioClip closeSound;  // Son à jouer lors de la fermeture

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isOpening = false;
    private bool isClosing = false;
    private float timer = 0f;  // Timer pour gérer le délai de fermeture
    private AudioSource audioSource;  // Référence à l'AudioSource
    private bool hasOpened = false;  // Pour jouer le son d'ouverture
    private bool hasClosed = false;  // Pour jouer le son de fermeture

    void Start()
    {
        // Stocke la position initiale de la porte
        initialPosition = transform.position;
        targetPosition = initialPosition + new Vector3(0, openHeight, 0);

        // Obtient la référence du composant AudioSource attaché à l'objet porte
        audioSource = GetComponent<AudioSource>();

        // Vérifie si l'AudioSource est bien assigné
        if (audioSource == null)
        {
            Debug.LogError("AudioSource n'a pas été trouvé sur l'objet porte !");
        }
    }
    public void openDoor()
    {

        // Calcule la distance entre le joueur et la porte
        float distance = Vector3.Distance(player.position, transform.position);

        // Si le joueur est dans la zone d'activation
        if (distance <= activationDistance)
        {
            // Si la porte n'est pas déjà en train d'ouvrir
            if (!isOpening && !isClosing)
            {
                isOpening = true;  // On ouvre la porte
                isClosing = false;  // La porte ne doit pas se fermer
                timer = 0f;  // Réinitialise le timer
                hasClosed = false;  // Réinitialise la fermeture

                // Jouer le son d'ouverture si ce n'est pas déjà fait
                if (!hasOpened && audioSource != null)
                {
                    audioSource.PlayOneShot(openSound); // Joue le son d'ouverture
                    hasOpened = true;  // Empêche de jouer le son plusieurs fois
                }
            }
        }

        if (isOpening)
        {
            // Ouvre la porte en utilisant Lerp
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * openSpeed);
        }

        // Si le joueur s'éloigne après avoir ouvert la porte
        if (distance > activationDistance)
        {
            // Démarre le timer pour attendre 3 secondes avant de fermer la porte
            timer += Time.deltaTime;

            // Si le délai est écoulé, on commence à fermer la porte
            if (timer >= closeDelay && isOpening) // Assurer que la porte est bien ouverte avant de fermer
            {
                isClosing = true;
                isOpening = false;  // Arrête l'ouverture
            }
        }

        if (isClosing)
        {
            // Joue le son de fermeture juste avant que la porte commence à se fermer
            if (!hasClosed && audioSource != null)
            {
                audioSource.PlayOneShot(closeSound); // Joue le son de fermeture
                hasClosed = true;  // Empêche de jouer le son plusieurs fois
            }

            // Ferme la porte en utilisant Lerp, mais cette fois on la ramène à sa position initiale
            transform.position = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * closeSpeed);

            // Vérifie si la porte est assez proche de sa position initiale pour l'arrêter
            if (Vector3.Distance(transform.position, initialPosition) < 0.05f)
            {
                transform.position = initialPosition;  // Force la position exacte
                isClosing = false;  // Arrête la fermeture une fois que la porte est fermée

                // Réinitialise hasOpened pour pouvoir rejouer le son d'ouverture lors du prochain passage
                hasOpened = false;  // Permet à la porte de rejouer le son d'ouverture
            }
        }

        // Si le joueur revient près de la porte, on arrête le timer et on garde la porte ouverte
        if (distance <= activationDistance)
        {
            timer = 0f;  // Réinitialise le timer si le joueur revient
            isClosing = false;  // Empêche la fermeture de la porte
            hasClosed = false;  // Réinitialise la fermeture pour pouvoir jouer le son la prochaine fois
        }
    }
    void Update()
    {
    }
}
