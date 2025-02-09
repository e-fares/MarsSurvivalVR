using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NavKeypad
{
    public class Keypad : MonoBehaviour
    {
        public Transform parentSpotLights;
        private List<GameObject> roomSpotlights;
        private GameObject[] roomLights;
        public ActivateAlarm playerScript;
        public TMPro.TextMeshProUGUI fixElectricity;


        [Header("Events")]
        [SerializeField] private UnityEvent onAccessGranted;
        [SerializeField] private UnityEvent onAccessDenied;

        [Header("Combination Code")]
        private string keypadCombo;
        public UnityEvent OnAccessGranted => onAccessGranted;
        public UnityEvent OnAccessDenied => onAccessDenied;

        [Header("Difficulty Settings")]
        public int codeLength = 4;
        public int totalLives = 5;
        public TMP_Text livesText;

        [Header("Secret Code Display")]
        public GameObject hiddenPlane;
        public TMP_Text planeText;

        [Header("Raw Image Display")]
        public GameObject rawImagePanel; // Assign the panel in Inspector
        public GameObject rawImagePrefab; // Assign a RawImage prefab in Inspector
        private List<GameObject> displayedRawImages = new List<GameObject>();

        [Header("Instruction Panel")]
        public TMP_Text instructionText;

        [Header("Settings")]
        [SerializeField] private string accessGrantedText = "Granted";
        [SerializeField] private string accessDeniedText = "Denied";

        [Header("Visuals")]
        [SerializeField] private float displayResultTime = 1f;
        [Range(0, 5)]
        [SerializeField] private float screenIntensity = 2.5f;
        [Header("Colors")]
        [SerializeField] private Color screenNormalColor = new Color(0.98f, 0.50f, 0.032f, 1f);
        [SerializeField] private Color screenDeniedColor = new Color(1f, 0f, 0f, 1f);
        [SerializeField] private Color screenGrantedColor = new Color(0f, 0.62f, 0.07f);
        [Header("SoundFx")]
        [SerializeField] private AudioClip buttonClickedSfx;
        [SerializeField] private AudioClip accessDeniedSfx;
        [SerializeField] private AudioClip accessGrantedSfx;
        [Header("Component References")]
        [SerializeField] private Renderer panelMesh;
        [SerializeField] private TMP_Text keypadDisplayText;
        [SerializeField] private AudioSource audioSource;

        private string currentInput;
        private bool displayingResult = false;
        private bool accessWasGranted = false;
        private List<Color> generatedColors = new List<Color>();

        private void Awake()
        {
            ClearInput();
            panelMesh.material.SetVector("_EmissionColor", screenNormalColor * screenIntensity);
        }

        private void Start()
        {
            GenerateCode();
            UpdateLivesUI();
            UpdateInstructionText();
        }

        public void SetDifficulty(int length, int lives)
        {
            codeLength = length;
            totalLives = lives;
            GenerateCode();
            UpdateLivesUI();
        }

        private void GenerateCode()
        {
            keypadCombo = "";
            generatedColors.Clear();

            // Define consistent colors for digits (0-9)
            Dictionary<int, Color> digitToColor = new Dictionary<int, Color>()
    {
        { 0, Color.red },
        { 1, Color.blue },
        { 2, Color.green },
        { 3, Color.yellow },
        { 4, Color.cyan },
        { 5, Color.magenta },
        { 6, Color.black },
        { 7, Color.white },
        { 8, Color.gray },
        { 9, new Color(1f, 0.5f, 0f) } // Orange
    };

            for (int i = 0; i < codeLength; i++)
            {
                int randomDigit = Random.Range(0, 10);
                keypadCombo += randomDigit.ToString();
                generatedColors.Add(digitToColor[randomDigit]); // Assign the correct color to the digit
            }

            Debug.Log("New Digicode: " + keypadCombo);

            // Ensure the plane displays the correct code
            if (planeText != null)
                planeText.text = "Code: " + keypadCombo;


            DisplayRawImagesForCode(); // Now colors will match the keypadCombo
        }

        private void DisplayRawImagesForCode()
        {
            // Clear previous images
            foreach (var img in displayedRawImages)
            {
                Destroy(img);
            }
            displayedRawImages.Clear();

            if (rawImagePanel != null)
            {
                rawImagePanel.SetActive(true); // Ensure panel is visible

                RectTransform panelRect = rawImagePanel.GetComponent<RectTransform>();
                float panelWidth = panelRect.rect.width;
                float spacing = 10f; // Space between images

                // Calculate size of each RawImage dynamically
                float imageSize = (panelWidth - (codeLength - 1) * spacing) / codeLength;

                for (int i = 0; i < codeLength; i++)
                {
                    GameObject newRawImage = Instantiate(rawImagePrefab, rawImagePanel.transform);
                    RawImage imageComponent = newRawImage.GetComponent<RawImage>();
                    RectTransform imageRect = newRawImage.GetComponent<RectTransform>();

                    if (imageComponent != null)
                    {
                        imageComponent.color = generatedColors[i]; // Now the colors will match the keypad digits
                    }

                    if (imageRect != null)
                    {
                        imageRect.sizeDelta = new Vector2(imageSize, imageSize);
                        imageRect.anchorMin = new Vector2(0, 0.5f);
                        imageRect.anchorMax = new Vector2(0, 0.5f);
                        imageRect.pivot = new Vector2(0, 0.5f);
                        imageRect.anchoredPosition = new Vector2(i * (imageSize + spacing), 0);
                    }

                    displayedRawImages.Add(newRawImage);
                }

            }
        }



        public void RotatePanelToRevealImages()
        {
            if (rawImagePanel != null)
            {
                rawImagePanel.SetActive(!rawImagePanel.activeSelf); // Toggle visibility
            }
        }

        public void AddInput(string input)
        {
            audioSource.PlayOneShot(buttonClickedSfx);
            if (displayingResult || accessWasGranted) return;

            switch (input)
            {
                case "enter":
                    CheckCombo();
                    break;
                default:
                    if (currentInput.Length >= codeLength)
                        return;

                    currentInput += input;
                    keypadDisplayText.text = currentInput;
                    break;
            }
        }

        public void CheckCombo()
        {
            if (currentInput == keypadCombo)
            {
                if (!displayingResult)
                {
                    StartCoroutine(DisplayResultRoutine(true));
                }
            }
            else
            {
                totalLives--;
                UpdateLivesUI();
                if (totalLives <= 0)
                {
                    Debug.Log("Out of Lives! Game Over.");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                else
                {
                    StartCoroutine(DisplayResultRoutine(false));
                }
            }
        }

        private IEnumerator DisplayResultRoutine(bool granted)
        {
            displayingResult = true;

            if (granted) AccessGranted();
            else AccessDenied();

            yield return new WaitForSeconds(displayResultTime);
            displayingResult = false;
            if (!granted)
            {
                ClearInput();
                panelMesh.material.SetVector("_EmissionColor", screenNormalColor * screenIntensity);
            }
        }

        private void AccessDenied()
        {
            keypadDisplayText.text = accessDeniedText;
            onAccessDenied?.Invoke();
            panelMesh.material.SetVector("_EmissionColor", screenDeniedColor * screenIntensity);
            audioSource.PlayOneShot(accessDeniedSfx);
        }

        private void ClearInput()
        {
            currentInput = "";
            keypadDisplayText.text = currentInput;
        }

        private void AccessGranted()
        {
            accessWasGranted = true;
            keypadDisplayText.text = accessGrantedText;
            onAccessGranted?.Invoke();
            panelMesh.material.SetVector("_EmissionColor", screenGrantedColor * screenIntensity);
            audioSource.PlayOneShot(accessGrantedSfx);
            Debug.Log("Access Granted!");
            Debug.Log("Electricity is now fixed!");

            playerScript.enabled = false;
            roomSpotlights = new List<GameObject>();
            for (int i = 0; i < parentSpotLights.childCount; i++)
            {
                Transform child = parentSpotLights.GetChild(i);
                roomSpotlights.Add(child.gameObject);
            }

            // Initialiser le tableau de la même taille que roomLights
            roomLights = new GameObject[roomSpotlights.Count];

            // Récupérer tous les enfants (un par parent)
            for (int i = 0; i < roomSpotlights.Count; i++)
            {
                if (roomSpotlights[i].transform.childCount > 0) // Vérifie si un enfant existe
                {
                    roomLights[i] = roomSpotlights[i].transform.GetChild(0).gameObject;
                }
            }

            foreach (GameObject light in roomLights)
            {
                light.GetComponent<Light>().color = Color.white;
                light.GetComponent<Light>().intensity = 2;
            }

            foreach (GameObject spotLight in roomSpotlights)
            {
                // Référence à l'objet
                Renderer renderer = spotLight.GetComponent<Renderer>();

                // Assurez-vous que le matériau utilise un shader supportant l'émission
                Material material = renderer.materials[1];

                // Assurez-vous que l'émission est activée
                material.EnableKeyword("_EMISSION");

                // Modifiez la couleur d'émission avec une interpolation
                material.SetColor("_EmissionColor", Color.white);
            }
            fixElectricity.color = ColorUtility.TryParseHtmlString("#4CFFB3", out Color newColor) ? newColor : fixElectricity.color;

        }

        private void UpdateLivesUI()
        {
            if (livesText != null)
            {
                livesText.text = "Lives Left: " + totalLives;
            }
        }

        private void UpdateInstructionText()
        {
            if (instructionText != null)
            {
                instructionText.text = "Accès Sécurisé – Instructions\n" +
                                       "Un coffre de sécurité est dissimulé à proximité.\n" +
                                       "Repérez les particules lumineuses qui l’entourent.\n" +
                                       "Approchez-vous et observez-le attentivement.\n" +
                                       "Retrouvez les chiffres qui correspondent au code couleur au dessus du keypad.\n\n" +
                                       "Restez vigilant et mémorisez-le.\n" +
                                       "Une fois saisi sur le terminal, l’accès sera débloqué.";
            }
        }

        private Color GetDistinctColor(int index)
        {
            Color[] colors = {
                Color.red, Color.blue, Color.green, Color.yellow, Color.cyan,
                Color.magenta, Color.black, Color.white, Color.gray, new Color(1f, 0.5f, 0f) // Orange
            };
            return colors[index % colors.Length];
        }
    }
}
