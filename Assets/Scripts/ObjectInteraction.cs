using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectInteraction : MonoBehaviour
{
    public Camera playerCamera; // R�f�rence � la cam�ra du joueur
    public float interactionRange = 3f; // Distance maximale pour interagir
    public LayerMask interactableLayer; // Pour filtrer les objets interactifs (si besoin)

    private void Update()
    {
        // Quand on appuie sur E (clic)
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteractClick();
        }
        // Quand on maintient E (maintien)
        else if (Input.GetKey(KeyCode.E))
        {
            TryInteractDown();
        }
        // Quand on rel�che E (fin du maintien)
        else if (Input.GetKeyUp(KeyCode.E))
        {
            TryInteractUp();
        }
    }

    // Fonction pour g�rer l'interaction lors d'un clic
    private void TryInteractClick()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // Rayon venant du centre de l'�cran
        // Si le rayon touche un objet dans la port�e et dans le layer interactable
        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            Debug.Log("Objet touch� (Click) : " + hitObject.name);
            // V�rifie si l'objet touch� a un EventTrigger
            EventTrigger trigger = hitObject.GetComponent<EventTrigger>();
            if (trigger != null)
            {
                // Simule un clic sur l'objet
                ExecuteEvents.Execute(hitObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                Debug.Log("Interaction (Click) avec " + hitObject.name);
            }
        }
    }

    // Fonction pour g�rer l'interaction lors du maintien du clic
    private void TryInteractDown()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // Rayon venant du centre de l'�cran
        // Si le rayon touche un objet dans la port�e et dans le layer interactable
        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            //Debug.Log("Objet touch� (Down) : " + hitObject.name);
            // V�rifie si l'objet touch� a un EventTrigger
            EventTrigger trigger = hitObject.GetComponent<EventTrigger>();
            if (trigger != null)
            {
                // Simule un appui sur l'objet
                ExecuteEvents.Execute(hitObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
                //Debug.Log("Interaction (Down) avec " + hitObject.name);
            }
        }
    }

    // Fonction pour g�rer l'interaction lors du rel�chement du clic
    private void TryInteractUp()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // Rayon venant du centre de l'�cran
        // Si le rayon touche un objet dans la port�e et dans le layer interactable
        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            //Debug.Log("Objet touch� (Up) : " + hitObject.name);
            // V�rifie si l'objet touch� a un EventTrigger
            EventTrigger trigger = hitObject.GetComponent<EventTrigger>();
            if (trigger != null)
            {
                // Simule un rel�chement sur l'objet
                ExecuteEvents.Execute(hitObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
                //Debug.Log("Interaction (Up) avec " + hitObject.name);
            }
        }
    }
}
