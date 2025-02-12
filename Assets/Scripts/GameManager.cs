using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
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

    // Référence au texte affichant le temps restant
    public TMP_Text timeText;
    public TMP_Text timeTextMinimap;

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
    public GameObject canvas;
    void Start()
    {
        alarmPlayed = false;
        timeRemaining = 99999;
        // Ajout des événements aux boutons de difficulté
        AddEventTrigger(easyButton, () => SelectDifficulty(15 * 60, easyButton));
        AddEventTrigger(mediumButton, () => SelectDifficulty(10 * 60, mediumButton));
        AddEventTrigger(hardButton, () => SelectDifficulty(7 * 60, hardButton));

        // Ajout de l'événement au bouton start
        AddEventTrigger(startButton, StartGame);
    }

    void Update()
    {
        // Si le timer a commencé et qu'il reste du temps, on décrémente le temps restant
        if (timerStarted && timeRemaining > -16)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimeText();
        }
        if (timerStarted && timeRemaining <= 0)
        {
            Debug.Log("Temps écoulé ! La base a explosé !");
            canvas.SetActive(true);
            alarm.StopAlarm();
        }
        if(timeRemaining<30 && !alarmPlayed)
        {
            Debug.Log(timeRemaining);
            alarm.PlayAlarm();
            alarmPlayed = true;
        }
        if (timeRemaining < -15 )
        {
            quitButton.QuitApplication();
        }
    }

    void SelectDifficulty(float time, EventTrigger selectedButton)
    {
        timeRemaining = time;
        UpdateTimeText();
        // Set difficulty settings for the digicode
        if (keypad != null)
        {
            if (selectedButton == easyButton)
            {
                keypad.SetDifficulty(4, 6); // 4-digit code, 5 lives
            }
            else if (selectedButton == mediumButton)
            {
                keypad.SetDifficulty(4, 5); // 6-digit code, 3 lives
            }
            else if (selectedButton == hardButton)
            {
                keypad.SetDifficulty(4, 4); // 8-digit code, 2 lives
            }
        }

    }

        void StartGame()
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
