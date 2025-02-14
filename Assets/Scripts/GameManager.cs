using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;
using System.Data;
 using UnityEngine.SceneManagement;
//using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    // Référence au digicode
    public NavKeypad.Keypad keypad;
    // Références aux boutons de sélection de difficulté
    public EventTrigger easyButton;
    public EventTrigger mediumButton;
    public EventTrigger hardButton;
    public EventTrigger startButton;
    public GameObject isColorBlind;
    // Référence au texte affichant le temps restant
    public TMP_Text timeText;
    public TMP_Text timeTextMinimap;
    public Planets planets;
    // Temps avant l'explosion de la base en secondes
    private float timeRemaining;
    private float timeRemaining2;
    private bool timerStarted = false;
    public QuitGameButton quitButton;
    // Référence à Door Controller
    public DoorController doorController;
    public ActivateAlarm alarm;
    // Référence à l'interface utilisateur
    public GameObject UILevelDifficulty;
    private bool alarmPlayed;
    public GameObject canvasLoss;
    public GameObject canvasVictory;
    public int MissionMax = 5;
    public int CurrentMissionCount=0;
    public bool isWin;
    public GameObject old_image;
    public GameObject new_image;
    public TMP_Text planetsText;
    void Start()
    {
        isWin = false;
        alarmPlayed = false;
        timeRemaining = 99999;


        // Ajout des événements aux boutons de difficulté
      /*  AddEventTrigger(easyButton, () => SelectDifficulty(15 * 60, easyButton));
        AddEventTrigger(mediumButton, () => SelectDifficulty(10 * 60, mediumButton));
        AddEventTrigger(hardButton, () => SelectDifficulty(7 * 60, hardButton));

        // Ajout de l'événement au bouton start
        AddEventTrigger(startButton, StartGame);*/
    }

    void Update()
    {
        // Si le timer a commencé et qu'il reste du temps, on décrémente le temps restant
        if (timerStarted && timeRemaining > -16)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimeText();
        }
        if (timerStarted && timeRemaining <= 0 && !isWin)
        {
            Debug.Log("Temps écoulé ! La base a explosé !");
            canvasLoss.SetActive(true);

            alarm.StopAlarm();
        }
        if(timeRemaining<30 && !alarmPlayed && !isWin)
        {

            alarm.PlayAlarm();
            alarmPlayed = true;
        }
        if (timeRemaining < -15 )
        {
         
             SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
          
        }
        CheckPlanetCount();
    }

    public void SelectDifficulty(float time, EventTrigger selectedButton)
    {
        timeRemaining = time;
        // Set difficulty settings for the digicode
        if (keypad != null)
        {
            if (selectedButton == easyButton)
            {
                keypad.SetDifficulty(4, 6, isColorBlind.activeInHierarchy); // 4-digit code, 5 lives
            }
            else if (selectedButton == mediumButton)
            {
                keypad.SetDifficulty(4, 5, isColorBlind.activeInHierarchy); // 6-digit code, 3 lives
            }
            else if (selectedButton == hardButton)
            {
                keypad.SetDifficulty(4, 4, isColorBlind.activeInHierarchy); // 8-digit code, 2 lives
            }
        }

    }
    public void SetDifficultyEasy(float time)
    {
        timeRemaining = time;
        UpdateTimeText();

        keypad.SetDifficulty(4, 6, isColorBlind.activeInHierarchy); // 4-digit code, 5 lives
        
    }
    public void SetDifficultyMedium(float time)
    {
        timeRemaining = time;
        UpdateTimeText();

        keypad.SetDifficulty(4, 5, isColorBlind.activeInHierarchy); // 6-digit code, 3 lives

    }
    public void SetDifficultyHard(float time)
    {
        timeRemaining = time;
        UpdateTimeText();

        keypad.SetDifficulty(4, 4, isColorBlind.activeInHierarchy); // 8-digit code, 2 lives

    }
    public void CheckPlanetCount()
    {
        planetsText.text = $"5. Reorder planets ({planets.GetPlanetCount()}/9)";

        if (planets.GetPlanetCount() == 9) { 
            Victory();
            old_image.SetActive(false);
            new_image.SetActive(true);
            planetsText.color = ColorUtility.TryParseHtmlString("#4CFFB3", out Color newColor) ? newColor : planetsText.color;

        }
    }
    public void Victory()
    {
        CurrentMissionCount += 1;
        if (CurrentMissionCount == MissionMax)
        {
            timeRemaining = 0;
            canvasVictory.SetActive(true);
            isWin = true;
        }
    }
        public void StartGame()
    {
        if (timeRemaining > 0 && !timerStarted)
        {
            // Démarrage du compte à rebours
            timerStarted = true;

            // Ouverture de la porte (activation ou désactivation selon la logique du jeu)
            UILevelDifficulty.SetActive(false);
            doorController.OpenDoor();
        }
    }

    void UpdateTimeText()
    {
        // Mise à jour du texte affichant le temps restant
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        timeTextMinimap.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

   

    void AddEventTrigger(EventTrigger trigger, System.Action action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => action());
        trigger.triggers.Add(entry);
    }
}
