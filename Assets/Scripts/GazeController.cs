using System.Collections;
using UnityEngine;

public class GazeController : MonoBehaviour
{
    public GameObject camera;
    public float maxAngleX;
    public float maxAngleY;
    private float slope;
    private float lastValue = -1;

    public GameObject crate; // Reference to the Crate object
    public string password;  // The password to display

    private bool isShaking = false;
    private Coroutine shakeCoroutine;
    private GameObject text3D;

    void Start()
    {
        Debug.Log(maxAngleX);
        slope = (0f - 1f) / (maxAngleX - 0f);
    }

    void Update()
    {
        if (camera == null || crate == null)
        {
            Debug.LogError("Camera or Crate is not assigned.");
            return;
        }

        Vector3 direction = camera.transform.forward;
        Vector3 cameraVec = transform.position - camera.transform.position;
        cameraVec.Normalize();
        float output = Vector3.Dot(direction, cameraVec);

        float distance = Vector3.Distance(camera.transform.position, crate.transform.position);

        if (output < 0.999f && distance <= 6.0f)
        {
            if (!isShaking && text3D == null)
            {
                shakeCoroutine = StartCoroutine(ShakeCrate());
            }
        }
        else
        {
            if (isShaking)
            {
                StopShake();
            }
        }
    }

    private IEnumerator ShakeCrate()
    {
        if (crate == null)
        {
            Debug.LogError("Crate is not assigned.");
            yield break;
        }

        isShaking = true;
        Vector3 originalPosition = crate.transform.position;
        float shakeDuration = 2.0f;
        float shakeMagnitude = 0.1f;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            crate.transform.position = originalPosition + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        crate.transform.position = originalPosition;
        isShaking = false;

        // Create the 3D text object
        text3D = new GameObject("PasswordText");
        text3D.transform.position = crate.transform.position + Vector3.up * 0.5f; // Position above crate

        // Add TextMesh component
        TextMesh textMesh = text3D.AddComponent<TextMesh>();
        textMesh.text = password;
        textMesh.fontSize = 15;
        textMesh.color = Color.yellow;
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;

        // Ensure camera is assigned
        if (camera == null)
        {
            Debug.LogError("Camera is not assigned.");
            yield break;
        }

        // Orient text towards the camera
        text3D.transform.LookAt(camera.transform);
        text3D.transform.Rotate(0, 180, 0);

        crate.SetActive(false);
    }

    private void StopShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }

        isShaking = false;

        // Destroy the 3D text if it exists
        if (text3D != null)
        {
            Destroy(text3D);
            text3D = null;
        }

        // Reactivate the crate
        crate.SetActive(true);
    }
}
