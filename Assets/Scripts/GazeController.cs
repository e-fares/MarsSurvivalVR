using System;
using System.Collections;
using UnityEngine;

public class GazeController : MonoBehaviour
{
    public GameObject camera;
    public GameObject crate;
    public string password;

    [Header("Gaze Detection Settings")]
    public float maxGazeAngle = 15f; // Max angle to consider the crate "looked at"
    public float maxDistance = 6.0f;  // Max distance for the crate to react

    [Header("Shake Effect")]
    public float shakeDuration = 2.0f;
    public float shakeMagnitude = 0.1f;

    private bool isShaking = false;
    private Coroutine shakeCoroutine;
    private GameObject text3D;
    private Vector3 originalCratePosition;

    /*public TMP_FontAsset customFont;*/
    void Start()
    {
        if (crate == null || camera == null)
        {
            Debug.LogError("Camera or Crate is not assigned.");
            return;
        }

        originalCratePosition = crate.transform.position;
    }

    void Update()
    {
        if (crate == null || camera == null) return;

        // Calculate gaze angle
        Vector3 toCrate = (crate.transform.position - camera.transform.position).normalized;
        float angle = Vector3.Angle(camera.transform.forward, toCrate);
        float distance = Vector3.Distance(camera.transform.position, crate.transform.position);

        // If looking at crate within range, start shaking
        if ( distance <= maxDistance)
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
        if (crate == null) yield break;

        isShaking = true;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            // Shake with a smooth Lerp movement
            Vector3 shakeOffset = new Vector3(
                UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude),
                UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude),
                UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude) * 0.5f
            );

            crate.transform.position = Vector3.Lerp(crate.transform.position, originalCratePosition + shakeOffset, Time.deltaTime * 10f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        crate.transform.position = originalCratePosition;
        isShaking = false;

        ShowPassword();
    }
    private void ShowPassword()
    {
        // Create the 3D text object
        text3D = new GameObject("PasswordText");

        // Position it above the crate
        text3D.transform.position = crate.transform.position + Vector3.up * 1.5f; // Raised more above the crate

        // Add TextMesh component
        TextMesh textMesh = text3D.AddComponent<TextMesh>();
        textMesh.text = password;
        textMesh.fontSize = 30; // Larger font size for better readability
        textMesh.color = Color.yellow;
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;

        // Make sure the text is always visible
        textMesh.characterSize = 0.1f; // Adjust text scale
        textMesh.fontStyle = FontStyle.Bold; // Bold text for better clarity
        textMesh.richText = true; // Ensure rich text support

        // Add an Outline for better visibility
        MeshRenderer textRenderer = text3D.GetComponent<MeshRenderer>();
        textRenderer.material.shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended"); // Ensures it's always visible

        // Make sure the text is always facing the player
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

        // Reset crate position
        crate.transform.position = originalCratePosition;
        crate.SetActive(true);
    }
}
