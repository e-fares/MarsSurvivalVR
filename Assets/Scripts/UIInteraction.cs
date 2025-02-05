using UnityEngine;
using UnityEngine.EventSystems;

public class UIInteraction : MonoBehaviour
{
    public Camera playerCamera; // Référence à la caméra du joueur
    public float interactionRange = 3f; // Distance maximale pour interagir
    public LayerMask interactableLayer; // Pour filtrer les objets interactifs (si besoin)

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Quand on appuie sur E
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // Rayon venant du centre de l'écran
        // Si le rayon touche un objet dans la portée et dans le layer interactable
        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            Debug.Log("Objet touché : " + hitObject.name);
            // Vérifie si l'objet touché a un EventTrigger
            EventTrigger trigger = hitObject.GetComponent<EventTrigger>();
            if (trigger != null)
            {
                // Si l'EventTrigger est trouvé, on simule le clic sur cet objet
                ExecuteEvents.Execute(hitObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                Debug.Log("Interaction avec " + hitObject.name);
            }
        }
    }
}