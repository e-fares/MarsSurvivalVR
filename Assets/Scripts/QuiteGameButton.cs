using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuiteGameButton : MonoBehaviour
{
    // Méthode pour quitter l'application
    public void QuitApplication()
    {
        // Vérifie si l'on est dans l'éditeur ou dans un build
#if UNITY_EDITOR
        // Si on est dans l'éditeur, arrêter le mode Play
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Quit in Editor");
#else
            // Si c'est un build, quitter l'application
            Debug.Log("Application Quit");
            Application.Quit();
#endif
    }
}
