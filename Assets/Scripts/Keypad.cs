using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NavKeypad
{
    public class Keypad : MonoBehaviour
    {
        public GameManager gameManager;
        public Transform parentSpotLights;
        private List<GameObject> roomSpotlights;
        private GameObject[] roomLights;
        public ActivateAlarm playerScript;
        public TMPro.TextMeshProUGUI fixElectricity;
        public GameObject prefButton;

        [Header("Events")]
        [SerializeField] private UnityEvent onAccessGranted;
        [SerializeField] private UnityEvent onAccessDenied;

        [Header("Combination Code")]
        private string keypadCombo;
        public UnityEvent OnAccessGranted => onAccessGranted;
        public UnityEvent OnAccessDenied => onAccessDenied;

        [Header("Difficulty Settings")]
        public int codeLength;
        public int totalLives;
        public bool isColorBlind;
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
        private Dictionary<int, Color> numberToColor;
        private string currentInput;
        private bool displayingResult = false;
        private bool accessWasGranted = false;
        private List<Color> generatedColors = new List<Color>();

        [Header("Logo Display")]
        public List<Sprite> numberLogos; // Assign 10 logo Sprites (0-9) in the Inspector

        private void DisplayLogosForCode()
        {
            foreach (var img in displayedRawImages)
                Destroy(img);
            displayedRawImages.Clear();

            if (rawImagePanel != null)
            {
                rawImagePanel.SetActive(true);
                RectTransform panelRect = rawImagePanel.GetComponent<RectTransform>();
                float panelWidth = panelRect.rect.width;
                float spacing = 10f;
                float imageSize = (panelWidth - (codeLength - 1) * spacing) / codeLength;

                List<int> digitOrder = new List<int>();
                foreach (char digit in keypadCombo)
                {
                    digitOrder.Add(int.Parse(digit.ToString()));
                }

                for (int i = 0; i < codeLength; i++)
                {
                    GameObject newRawImage = Instantiate(rawImagePrefab, rawImagePanel.transform);
                    RawImage imageComponent = newRawImage.GetComponent<RawImage>();
                    RectTransform imageRect = newRawImage.GetComponent<RectTransform>();

                    int digit = digitOrder[i];

                    if (imageComponent != null && numberLogos.Count > digit)
                    {
                        imageComponent.texture = numberLogos[digit].texture; // Assign logo instead of color

                        // Force Square Aspect Ratio
                        imageComponent.rectTransform.sizeDelta = new Vector2(imageSize, imageSize);
                        imageComponent.rectTransform.localScale = Vector3.one;
                        imageComponent.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                        imageComponent.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                        imageComponent.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    }
                    else
                    {
                        Debug.LogWarning("Not enough logos assigned in numberLogos list!");
                    }

                    if (imageRect != null)
                    {
                        PositionRawImage(imageRect, i, imageSize, spacing);
                    }

                    displayedRawImages.Add(newRawImage);
                }
            }
        }

        private void Awake()
        {
            InitializeColorMapping();

            ClearInput();
            panelMesh.material.SetVector("_EmissionColor", screenNormalColor * screenIntensity);
        }

        private void Start()
        {
            BoxCollider[] colliders = prefButton.GetComponentsInChildren<BoxCollider>();

            foreach (BoxCollider col in colliders)
            {
                col.enabled = true;
            }
            UpdateInstructionText();
        }

        public void SetDifficulty(int length, int lives, bool colorBlind)
        {

            codeLength = length;
            totalLives = lives;
            isColorBlind = colorBlind;
            GenerateCode();
            UpdateLivesUI();
        }
        public void reduceLife()
        {
            totalLives--;
            if (totalLives == 0)
            {
                BoxCollider[] colliders = prefButton.GetComponentsInChildren<BoxCollider>();

                foreach (BoxCollider col in colliders)
                {
                    col.enabled = false;
                }

            }
            UpdateLivesUI();

        }
        private void GenerateCode()
        {
            keypadCombo = "";

            for (int i = 0; i < codeLength; i++)
            {
                int randomDigit = Random.Range(0, 10);
                keypadCombo += randomDigit.ToString();
            }

            Debug.Log("Generated Code: " + keypadCombo);

            if (planeText != null)
                planeText.text = "Code: " + keypadCombo;
            if(isColorBlind)
            DisplayRawImagesForCode();
            else
                DisplayLogosForCode();
        }

        private void InitializeColorMapping()
        {
            numberToColor = new Dictionary<int, Color>
        {
            { 0, Color.red },
            { 1, Color.blue },
            { 2, Color.green },
            { 3, Color.yellow },
            { 4, Color.cyan },
            { 5, Color.magenta },
            { 6, Color.black },
            { 7, new Color(65/255.0f,42/255.0f,42/255.0f)},
            { 8, Color.gray },
            { 9, new Color(1f, 0.5f, 0f) } // Orange
        };
        }

        private void DisplayRawImagesForCode()
        {
            foreach (var img in displayedRawImages)
                Destroy(img);
            displayedRawImages.Clear();

            if (rawImagePanel != null)
            {
                rawImagePanel.SetActive(true);
                RectTransform panelRect = rawImagePanel.GetComponent<RectTransform>();
                float panelWidth = panelRect.rect.width;
                float spacing = 10f;
                float imageSize = (panelWidth - (codeLength - 1) * spacing) / codeLength;

                List<int> digitOrder = new List<int>();
                foreach (char digit in keypadCombo)
                {
                    digitOrder.Add(int.Parse(digit.ToString()));
                }

                for (int i = 0; i < codeLength; i++)
                {
                    GameObject newRawImage = Instantiate(rawImagePrefab, rawImagePanel.transform);
                    RawImage imageComponent = newRawImage.GetComponent<RawImage>();
                    RectTransform imageRect = newRawImage.GetComponent<RectTransform>();

                    int digit = digitOrder[i];

                    if (imageComponent != null)
                        imageComponent.color = numberToColor[digit];

                    if (imageRect != null)
                    {
                        PositionRawImage(imageRect, i, imageSize, spacing);
                    }

                    displayedRawImages.Add(newRawImage);
                }
            }
        }

        private void PositionRawImage(RectTransform imageRect, int index, float imageSize, float spacing)
        {
            // Ensure correct anchoring and pivot settings
            imageRect.anchorMin = new Vector2(0, 0.5f);
            imageRect.anchorMax = new Vector2(0, 0.5f);
            imageRect.pivot = new Vector2(0, 0.5f);

            // Position images correctly with spacing
            imageRect.sizeDelta = new Vector2(imageSize, imageSize);
            imageRect.anchoredPosition = new Vector2(index * (imageSize + spacing), 0);
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
                StartCoroutine(DisplayResultRoutine(false));
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
            gameManager.Victory();
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
                instructionText.text = "Pour reactiver l'electricite, entrez le bon code.\n" +
                                       "L'acces a ce dernier est sécurise.\r\n\n" +
                                       "Approchez-vous du coffre de securite situe a proximite\r\n\n" +
                                       "et choisissez les bonnes couleurs\r\n\n" +
                                       "Cela vous revelera la combinaison correcte..\n\n";
                                       
            }
        }


    }
  
}


    