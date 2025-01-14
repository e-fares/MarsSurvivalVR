using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class ActivateAlarm : MonoBehaviour
{
    // Liste des lumières à modifier
    public GameObject[] roomLights;

    // Durée totale du cycle (secondes)
    public float duration = 1f;

    // Durée de l'effet (en secondes)
    public float effectTime = 20f;

    // Temps écoulé pour l'effet complet
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
