using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.UI;
public class VRDoorHandle : MonoBehaviour
{
    public float handleRotationLimit = 45f; // Max rotation for handle (up/down)

    public GameManager gameManager;
    private XRGrabInteractable grabInteractable;
    private Quaternion initialHandleRotation;
    private bool isHandleActivated = false;
    private float targetDoorRotation;
    public TMP_Text doorHandles;
    int countDoor = 0;
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.onSelectExited.AddListener(HandleReleased);
        initialHandleRotation = transform.localRotation;
    }

    void Update()
    {
      
    }

    public void HandleReleased(XRBaseInteractor interactor)
    {
        float handleAngle = Quaternion.Angle(initialHandleRotation, transform.localRotation);

        // Check if the handle was moved enough to trigger the door
        if (handleAngle > handleRotationLimit * 0.5f)
        {
            isHandleActivated = true;
            countDoor++;
        }
        doorHandles.text = $"3. Unlock door ({countDoor}/2)";
        if(countDoor ==2)
        {
            doorHandles.color = ColorUtility.TryParseHtmlString("#4CFFB3", out Color newColor) ? newColor : doorHandles.color;
        }



        // Reset the handle position
        transform.localRotation = initialHandleRotation;
    }
}
