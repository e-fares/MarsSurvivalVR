using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuiteGameButton : MonoBehaviour
{
    // M�thode pour quitter l'application
    public void QuitApplication()
    {
        // V�rifie si l'on est dans l'�diteur ou dans un build
#if UNITY_EDITOR
        // Si on est dans l'�diteur, arr�ter le mode Play
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Quit in Editor");
#else
            // Si c'est un build, quitter l'application
            Debug.Log("Application Quit");
            Application.Quit();
#endif
    }
}
