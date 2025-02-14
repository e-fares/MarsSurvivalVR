using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OxygenBottle : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private bool canGrab = false;
    public GameObject leftHand;
    private void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.enabled = false; // Disable grabbing by default
            
            grabInteractable.selectEntered.AddListener(OnGrabbed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (leftHand) // Check for hand controllers
        {
            canGrab = true;
            grabInteractable.enabled = true; // Enable grab when hand is in range
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (leftHand) // Check if hand leaves range
        {
            canGrab = false;
            grabInteractable.enabled = false; // Disable grab when hand moves away
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (!canGrab)
        {
            grabInteractable.interactionManager.CancelInteractableSelection(grabInteractable);
        }
    }
}
