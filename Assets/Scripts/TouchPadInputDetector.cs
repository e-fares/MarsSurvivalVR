
using UnityEngine;
using UnityEngine.XR;

public class TouchPadInputDetector : MonoBehaviour
{
    private InputDevice rightHandDevice;
    private InputDevice leftHandDevice;

    private bool isRightTouchpadPressed = false;
    private bool isLeftTouchpadPressed = false;
    private Vector2 rightTouchpadPosition = Vector2.zero;
    private Vector2 leftTouchpadPosition = Vector2.zero;
    public GameObject PoleController;
    public GameObject UIController;
    void Start()
    {
        // Get the devices for the right and left hands
      //  rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
    }

    void Update()
    {
        // Check if the devices are valid
      /*  if (!rightHandDevice.isValid || !leftHandDevice.isValid)
        {
            Debug.LogError("One or more XR devices are not valid.");
            return;
        }*/

        // Detect Right Touchpad Press
      /*  bool rightTouchpadPressed;
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out rightTouchpadPressed))
        {
            if (rightTouchpadPressed && !isRightTouchpadPressed)
            {
                Debug.Log("Right touchpad pressed");
            }
            isRightTouchpadPressed = rightTouchpadPressed;
        }
*/
        // Detect Left Touchpad Press
        bool leftTouchpadPressed;
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out leftTouchpadPressed))
        {
            if (leftTouchpadPressed && !isLeftTouchpadPressed)
            {
                
                //Debug.Log("Left touchpad pressed");
            }
            isLeftTouchpadPressed = leftTouchpadPressed;
        }

      /*  // Detect Right Touchpad Position (Touch)
        if (rightHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out rightTouchpadPosition))
        {
            if (rightTouchpadPosition != Vector2.zero)
            {
                Debug.Log("Right touchpad position: " + rightTouchpadPosition);
            }
        }*/

        // Detect Left Touchpad Position (Touch)
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out leftTouchpadPosition))
        {
            if (leftTouchpadPosition.y >  0.5f)
            {
                PoleController.SetActive(false);
                UIController.SetActive(true);
            }
            else if (leftTouchpadPosition.y < 0.5f)
                {
                    PoleController.SetActive(true);
                    UIController.SetActive(false);
                }
        }
    }
}
