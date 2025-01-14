using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class ActivateAlarm : MonoBehaviour
{
    // Liste des lumi�res � modifier
    public GameObject[] roomLights;

    // Dur�e totale du cycle (secondes)
    public float duration = 1f;

    // Dur�e de l'effet (en secondes)
    public float effectTime = 20f;

    // Temps �coul� pour l'effet complet
    private float effectElapsedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (effectElapsedTime < effectTime)
        {
            float t = Mathf.PingPong(Time.time, 1f);

            foreach (GameObject light in roomLights)
            {
                light.GetComponent<Light>().color = Color.Lerp(Color.red, Color.white, t);
            }
            effectElapsedTime += Time.deltaTime;
        }
        else
        {
            foreach (GameObject light in roomLights)
            {
                light.GetComponent<Light>().color = Color.white;
            }
            this.enabled = false;
        }
    }
}
