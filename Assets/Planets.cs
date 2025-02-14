using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class Planets : MonoBehaviour
{
    public string correctSlotName; // Assign the correct slot in Inspector
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isPlaced = false;
    private XRGrabInteractable grabInteractable;
    public static int planetCount = 0;
    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPlaced) return; // Prevent checking if already placed

        if (other.CompareTag("PlanetSlot")) // Check if colliding with a slot
        {
            if (other.gameObject.name == correctSlotName) // Correct slot
            {
                PlacePlanet(other.gameObject);
            }
            else // Wrong slot
            {
                StartCoroutine(FlashRed(other.gameObject));
            }
        }
    }

    private void PlacePlanet(GameObject slot)
    {
        transform.position = slot.transform.position;
        transform.rotation = slot.transform.rotation;
        grabInteractable.enabled = false; // Disable grabbing
        isPlaced = true;

        slot.SetActive(false); // Hide the slot
        planetCount++;
    }
    public int GetPlanetCount()
    {

    return planetCount; }
    private IEnumerator FlashRed(GameObject slot)
    {
        Renderer slotRenderer = slot.GetComponent<Renderer>();
        if (slotRenderer != null)
        {
            Color originalColor = slotRenderer.material.color;
            slotRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            slotRenderer.material.color = originalColor;
        }
    }
}
