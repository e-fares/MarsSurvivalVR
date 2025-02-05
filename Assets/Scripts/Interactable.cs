using UnityEngine;
using UnityEngine.EventSystems;

public class Interactable : MonoBehaviour
{
    public Camera playerCamera; // R�f�rence � la cam�ra du joueur
    public float interactionRange = 3f; // Distance maximale pour interagir
    public LayerMask interactableLayer = ~0; // V�rifie sur tous les layers

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
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            Debug.Log("Objet touch� : " + hitObject.name);

            IInteractable interactable = hitObject.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Debug.Log("Interaction avec : " + hitObject.name);
                interactable.Interact();
            }
            else
            {
                Debug.LogWarning("L'objet touch� n'a pas de script IInteractable.");
            }
        }
        else
        {
            Debug.Log("Aucun objet interactif touch�.");
        }
    }

}

// Interface pour tous les objets interactifs
public interface IInteractable
{
    void Interact();
}
