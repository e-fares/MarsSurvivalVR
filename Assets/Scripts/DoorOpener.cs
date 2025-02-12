using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public Transform player;  // R�f�rence au joueur
    public float activationDistance = 3f;  // Distance pour ouvrir la porte
    public float openHeight = 3f;  // Hauteur � laquelle la porte s'ouvre
    public float openSpeed = 2f;  // Vitesse d'ouverture
    public float closeSpeed = 2f; // Vitesse de fermeture
    public float closeDelay = 3f;  // D�lai avant de fermer la porte
    public AudioClip openSound;   // Son � jouer lors de l'ouverture
    public AudioClip closeSound;  // Son � jouer lors de la fermeture

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isOpening = false;
    private bool isClosing = false;
    private float timer = 0f;  // Timer pour g�rer le d�lai de fermeture
    private AudioSource audioSource;  // R�f�rence � l'AudioSource
    private bool hasOpened = false;  // Pour jouer le son d'ouverture
    private bool hasClosed = false;  // Pour jouer le son de fermeture

    void Start()
    {
        // Stocke la position initiale de la porte
        initialPosition = transform.position;
        targetPosition = initialPosition + new Vector3(0, openHeight, 0);

        // Obtient la r�f�rence du composant AudioSource attach� � l'objet porte
        audioSource = GetComponent<AudioSource>();

        // V�rifie si l'AudioSource est bien assign�
        if (audioSource == null)
        {
            Debug.LogError("AudioSource n'a pas �t� trouv� sur l'objet porte !");
        }
    }
    public void openDoor()
    {

        // Calcule la distance entre le joueur et la porte
        float distance = Vector3.Distance(player.position, transform.position);

        // Si le joueur est dans la zone d'activation
        if (distance <= activationDistance)
        {
            // Si la porte n'est pas d�j� en train d'ouvrir
            if (!isOpening && !isClosing)
            {
                isOpening = true;  // On ouvre la porte
                isClosing = false;  // La porte ne doit pas se fermer
                timer = 0f;  // R�initialise le timer
                hasClosed = false;  // R�initialise la fermeture

                // Jouer le son d'ouverture si ce n'est pas d�j� fait
                if (!hasOpened && audioSource != null)
                {
                    audioSource.PlayOneShot(openSound); // Joue le son d'ouverture
                    hasOpened = true;  // Emp�che de jouer le son plusieurs fois
                }
            }
        }

        if (isOpening)
        {
            // Ouvre la porte en utilisant Lerp
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * openSpeed);
        }

        // Si le joueur s'�loigne apr�s avoir ouvert la porte
        if (distance > activationDistance)
        {
            // D�marre le timer pour attendre 3 secondes avant de fermer la porte
            timer += Time.deltaTime;

            // Si le d�lai est �coul�, on commence � fermer la porte
            if (timer >= closeDelay && isOpening) // Assurer que la porte est bien ouverte avant de fermer
            {
                isClosing = true;
                isOpening = false;  // Arr�te l'ouverture
            }
        }

        if (isClosing)
        {
            // Joue le son de fermeture juste avant que la porte commence � se fermer
            if (!hasClosed && audioSource != null)
            {
                audioSource.PlayOneShot(closeSound); // Joue le son de fermeture
                hasClosed = true;  // Emp�che de jouer le son plusieurs fois
            }

            // Ferme la porte en utilisant Lerp, mais cette fois on la ram�ne � sa position initiale
            transform.position = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * closeSpeed);

            // V�rifie si la porte est assez proche de sa position initiale pour l'arr�ter
            if (Vector3.Distance(transform.position, initialPosition) < 0.05f)
            {
                transform.position = initialPosition;  // Force la position exacte
                isClosing = false;  // Arr�te la fermeture une fois que la porte est ferm�e

                // R�initialise hasOpened pour pouvoir rejouer le son d'ouverture lors du prochain passage
                hasOpened = false;  // Permet � la porte de rejouer le son d'ouverture
            }
        }

        // Si le joueur revient pr�s de la porte, on arr�te le timer et on garde la porte ouverte
        if (distance <= activationDistance)
        {
            timer = 0f;  // R�initialise le timer si le joueur revient
            isClosing = false;  // Emp�che la fermeture de la porte
            hasClosed = false;  // R�initialise la fermeture pour pouvoir jouer le son la prochaine fois
        }
    }
    void Update()
    {
    }
}
