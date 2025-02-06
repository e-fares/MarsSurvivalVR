using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
//using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    // Références aux boutons de sélection de difficulté
    public EventTrigger easyButton;
    public EventTrigger mediumButton;
    public EventTrigger hardButton;
    public EventTrigger startButton;

    // Référence au texte affichant le temps restant
    public TMP_Text timeText;

    // Temps avant l'explosion de la base en secondes
    private float timeRemaining;
    private bool timerStarted = false;

    // Référence à Door Controller
    public DoorController doorController;

    // Référence à l'interface utilisateur
    public GameObject UILevelDifficulty;

    void Start()
    {
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
        if (timerStarted && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimeText();
        }
        else if (timerStarted && timeRemaining <= 0)
        {
            timeRemaining = 0;
            timerStarted = false;
            Debug.Log("Temps écoulé ! La base a explosé !");
        }
    }

    void SelectDifficulty(float time, EventTrigger selectedButton)
    {
        // Mise à jour du temps de jeu
        timeRemaining = time;
        UpdateTimeText(); // Ensure UI updates immediately

        // Optionally highlight the selected button (not required but can be useful)
        Debug.Log($"Difficulty selected: {timeRemaining} seconds");

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
    }

   

    void AddEventTrigger(EventTrigger trigger, System.Action action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => action());
        trigger.triggers.Add(entry);
    }
}
