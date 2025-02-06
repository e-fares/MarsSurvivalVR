using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
//using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    // R�f�rences aux boutons de s�lection de difficult�
    public EventTrigger easyButton;
    public EventTrigger mediumButton;
    public EventTrigger hardButton;
    public EventTrigger startButton;

    // R�f�rence au texte affichant le temps restant
    public TMP_Text timeText;

    // Temps avant l'explosion de la base en secondes
    private float timeRemaining;
    private bool timerStarted = false;

    // R�f�rence � Door Controller
    public DoorController doorController;

    // R�f�rence � l'interface utilisateur
    public GameObject UILevelDifficulty;

    void Start()
    {
        // Ajout des �v�nements aux boutons de difficult�
        AddEventTrigger(easyButton, () => SelectDifficulty(15 * 60, easyButton));
        AddEventTrigger(mediumButton, () => SelectDifficulty(10 * 60, mediumButton));
        AddEventTrigger(hardButton, () => SelectDifficulty(7 * 60, hardButton));

        // Ajout de l'�v�nement au bouton start
        AddEventTrigger(startButton, StartGame);
    }

    void Update()
    {
        // Si le timer a commenc� et qu'il reste du temps, on d�cr�mente le temps restant
        if (timerStarted && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimeText();
        }
        else if (timerStarted && timeRemaining <= 0)
        {
            timeRemaining = 0;
            timerStarted = false;
            Debug.Log("Temps �coul� ! La base a explos� !");
        }
    }

    void SelectDifficulty(float time, EventTrigger selectedButton)
    {
        // Mise � jour du temps de jeu
        timeRemaining = time;
        UpdateTimeText(); // Ensure UI updates immediately

        // Optionally highlight the selected button (not required but can be useful)
        Debug.Log($"Difficulty selected: {timeRemaining} seconds");

    }


    void StartGame()
    {
        if (timeRemaining > 0 && !timerStarted)
        {
            // D�marrage du compte � rebours
            timerStarted = true;

            // Ouverture de la porte (activation ou d�sactivation selon la logique du jeu)
            UILevelDifficulty.SetActive(false);
            doorController.OpenDoor();
        }
    }

    void UpdateTimeText()
    {
        // Mise � jour du texte affichant le temps restant
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
