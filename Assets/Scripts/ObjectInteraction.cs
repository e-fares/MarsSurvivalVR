using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectInteraction : MonoBehaviour
{
    public Camera playerCamera; // Référence à la caméra du joueur
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
        // Quand on relâche E (fin du maintien)
        else if (Input.GetKeyUp(KeyCode.E))
        {
            TryInteractUp();
        }
    }

    // Fonction pour gérer l'interaction lors d'un clic
    private void TryInteractClick()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // Rayon venant du centre de l'écran
        // Si le rayon touche un objet dans la portée et dans le layer interactable
        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            Debug.Log("Objet touché (Click) : " + hitObject.name);
            // Vérifie si l'objet touché a un EventTrigger
            EventTrigger trigger = hitObject.GetComponent<EventTrigger>();
            if (trigger != null)
            {
                // Simule un clic sur l'objet
                ExecuteEvents.Execute(hitObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                Debug.Log("Interaction (Click) avec " + hitObject.name);
            }
        }
    }

    // Fonction pour gérer l'interaction lors du maintien du clic
    private void TryInteractDown()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // Rayon venant du centre de l'écran
        // Si le rayon touche un objet dans la portée et dans le layer interactable
        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            //Debug.Log("Objet touché (Down) : " + hitObject.name);
            // Vérifie si l'objet touché a un EventTrigger
            EventTrigger trigger = hitObject.GetComponent<EventTrigger>();
            if (trigger != null)
            {
                // Simule un appui sur l'objet
                ExecuteEvents.Execute(hitObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
                //Debug.Log("Interaction (Down) avec " + hitObject.name);
            }
        }
    }

    // Fonction pour gérer l'interaction lors du relâchement du clic
    private void TryInteractUp()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // Rayon venant du centre de l'écran
        // Si le rayon touche un objet dans la portée et dans le layer interactable
        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            //Debug.Log("Objet touché (Up) : " + hitObject.name);
            // Vérifie si l'objet touché a un EventTrigger
            EventTrigger trigger = hitObject.GetComponent<EventTrigger>();
            if (trigger != null)
            {
                // Simule un relâchement sur l'objet
                ExecuteEvents.Execute(hitObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
                //Debug.Log("Interaction (Up) avec " + hitObject.name);
            }
        }
    }
}
