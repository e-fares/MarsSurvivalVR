using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.PXR;
using TMPro;
using UnityEngine.XR;
public class TouchPadController : MonoBehaviour
{
    public GameObject UIInteractor;
    public GameObject teleportInteractor;
    public GameObject locomotionSystem;
    TeleportationArea[] teleportationAreas = new TeleportationArea[99];
    private InputDevice leftController;
    void Start()
    {
         teleportationAreas = FindObjectsOfType<TeleportationArea>();

        leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        EnableUIInteraction();

    }
    
  

// Update is called once per frame
void Update()
    {

        if (leftController.isValid)
        {
            bool touchpadClicked = false;
            Vector2 touchpadInput = Vector2.zero;

            if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out touchpadClicked) && touchpadClicked)
            {
               
                leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out touchpadInput);


                if (touchpadInput.y > 0.5f)
            {
                EnableUIInteraction();
            }
            else if (touchpadInput.y < -0.5f)
            {
                EnableTeleportation();
            }
        }
    }
}
    private void EnableUIInteraction()
    {
        UIInteractor.SetActive(true);
        teleportInteractor.SetActive(false);
        locomotionSystem.SetActive(false);
        foreach (var area in teleportationAreas)
        {
            area.enabled = false;  // Disable the script
        }

        Debug.Log("All Teleportation Areas have been deactivated.");
    }

    private void EnableTeleportation()
    {
        UIInteractor.SetActive(false);
        teleportInteractor.SetActive(true);
        locomotionSystem.SetActive(true);
    foreach (var area in teleportationAreas)
    {
        area.enabled = true;  // Enable the script
    }

    Debug.Log("All Teleportation Areas have been activated.");
}

   

}
