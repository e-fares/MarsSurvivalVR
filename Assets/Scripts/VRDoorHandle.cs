using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class VRDoorHandle : MonoBehaviour
{
    public float handleRotationLimit = 45f;
    public GameManager gameManager;
    public TMP_Text doorHandles;
    private Quaternion initialHandleRotation;
    private bool isReturning = false;
    public static int countDoor = 0;

    void Start()
    {
        initialHandleRotation = transform.localRotation;
    }
    public int getCount()
    {

    return countDoor; }

    void Update()
    {
        if (isReturning)
        {
            float rotationSpeed = 200f;  // Adjust for smoothness
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, initialHandleRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.localRotation, initialHandleRotation) < 0.1f)
            {
                transform.localRotation = initialHandleRotation;
                isReturning = false;
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    public void HandleReleased()
    {
        isReturning = true;  // Trigger the return rotation

        countDoor++;
        doorHandles.text = $"3. Unlock door ({countDoor}/2)";

        if (countDoor == 2)
        {
            countDoor = 5000;
            doorHandles.color = ColorUtility.TryParseHtmlString("#4CFFB3", out Color newColor) ? newColor : doorHandles.color;
            gameManager.Victory();
        }
        
    }
}
