using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;

public class DigiCodePanelBlonde : MonoBehaviour
{
    public GameObject rawImagePrefab; // Prefab for RawImage
    public TextMeshProUGUI instructionText; // Instruction text (TextMeshPro)
    public List<Sprite> blondePNGs; // Assign 10 PNGs in the Inspector

    private Dictionary<int, GameObject> rawImages = new Dictionary<int, GameObject>();
    private List<int> availableNumbers = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    private NavKeypad.Keypad keypad;
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

            // Assign PNG Sprite instead of color
            if (blondePNGs.Count > i)
            {
                rawImage.texture = blondePNGs[i].texture;
            }
            else
            {
                Debug.LogWarning("Not enough PNGs assigned in blondePNGs list!");
            }

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
    }
}
