using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;
public class DigiCodePanel : MonoBehaviour
{
    public GameObject rawImagePrefab; // Prefab for RawImage
    public TextMeshProUGUI instructionText; // Instruction text (TextMeshPro)
    private Dictionary<int, GameObject> rawImages = new Dictionary<int, GameObject>();
    private List<int> availableNumbers = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    private List<Color> distinctColors = new List<Color>
    {
        Color.red, Color.blue, Color.green, Color.yellow, Color.cyan,
        Color.magenta, Color.black, new Color(65/255.0f,42/255.0f,42/255.0f) ,Color.gray, new Color(1.0f, 0.5f, 0f) // Orange
    };
    private NavKeypad.Keypad keypad;
    public TextMeshProUGUI livesText; // Assign in Inspector
    public TMP_FontAsset textFont; // Assign a custom font in Inspector

    void Start()
    {
       
            keypad = FindObjectOfType<NavKeypad.Keypad>(); 

            if (keypad == null)
            {
                Debug.LogError("Keypad not found in scene!");
            }

            CreateInstructionText();
        GenerateRawImages();
    }

    void CreateInstructionText()
    {
        if (instructionText != null)
        {
            instructionText.text = "Sélectionne les bonnes images pour trouver ton code";
        }
    }

    void GenerateRawImages()
    {
        int columns = 5; // 5 columns
        int rows = 2; // 2 rows
        float spacing = 10f; // Space between images

        RectTransform panelRect = GetComponent<RectTransform>();
        float panelWidth = panelRect.rect.width;
        float panelHeight = panelRect.rect.height;

        // Calculate the max square size that fits within the panel
        float squareSize = Mathf.Min(
            (panelWidth - (columns + 1) * spacing) / columns,
            (panelHeight - (rows + 1) * spacing) / rows
        );

        // Create a list of positions
        List<Vector2> positions = new List<Vector2>();

        for (int i = 0; i < 10; i++)
        {
            int row = i / columns;
            int col = i % columns;
            Vector2 position = new Vector2(
                spacing + col * (squareSize + spacing),
                -spacing - row * (squareSize + spacing)
            );
            positions.Add(position);
        }

        positions.Shuffle(); 

        for (int i = 0; i < 10; i++)
        {
            GameObject rawImageGO = Instantiate(rawImagePrefab, transform);
            RawImage rawImage = rawImageGO.GetComponent<RawImage>();

            RectTransform rawImageRect = rawImageGO.GetComponent<RectTransform>();
            rawImageRect.SetParent(panelRect, false); // Ensure it's inside the panel
            rawImageRect.anchorMin = new Vector2(0, 1); // Top-left anchoring
            rawImageRect.anchorMax = new Vector2(0, 1);
            rawImageRect.pivot = new Vector2(0, 1); // Pivot at top-left
            rawImageRect.sizeDelta = new Vector2(squareSize, squareSize);

            // Assign shuffled position
            rawImageRect.anchoredPosition = positions[i];

            int number = availableNumbers[i];
            rawImage.color = distinctColors[i];

            // Add XR Interaction
            XRSimpleInteractable interactable = rawImageGO.AddComponent<XRSimpleInteractable>();
            interactable.selectEntered.AddListener((interactor) => OnImageClick(rawImageGO, number));

            // Add Event Trigger for UI interactions
            EventTrigger eventTrigger = rawImageGO.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entry.callback.AddListener((eventData) => OnImageClick(rawImageGO, number));
            eventTrigger.triggers.Add(entry);

            rawImages[number] = rawImageGO; // Keeping GameObject for flexibility
        }
    }


    void OnImageClick(GameObject imageGO, int number)
    {
        // Reduce lives if it's a wrong choice
        keypad.reduceLife();
        // Create or find the TextMeshProUGUI component
        TextMeshProUGUI buttonText = imageGO.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText == null)
        {
            GameObject textGO = new GameObject("ButtonText");
            textGO.transform.SetParent(imageGO.transform, false);
            textGO.AddComponent<CanvasRenderer>();

            buttonText = textGO.AddComponent<TextMeshProUGUI>();
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.white;
            buttonText.fontSize = 18;
            if (textFont != null) buttonText.font = textFont;

            RectTransform textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
        }

        buttonText.text = number.ToString();

        // Animate Flip and Disappear
        StartCoroutine(FlipAndDisappear(imageGO));
       
    }

    // UI Update for Lives


    // Coroutine for Flip & Disappear
    IEnumerator FlipAndDisappear(GameObject imageGO)
    {
        imageGO.GetComponent<BoxCollider>().enabled = false;
        RectTransform rectTransform = imageGO.GetComponent<RectTransform>();

        // Flip animation (Rotate 360 degrees over time)
        for (float i = 0; i <= 360; i += 10)
        {
            rectTransform.localRotation = Quaternion.Euler(0, i, 0);
            yield return new WaitForSeconds(0.02f);
        }

        // Hide image after flip
        //imageGO.SetActive(false); // keep the raw image
    }
}

// Shuffle helper for lists
public static class ListExtensions
{
    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}
