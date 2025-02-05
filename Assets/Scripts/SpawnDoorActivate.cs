using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnDoorActivate : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private float openHeight = 3f;
    private float openSpeed = 2f;

    public void OpenDoor()
    {
        Debug.Log("cc");
        // Stocke la position initiale de la porte
        initialPosition = transform.position;
        targetPosition = initialPosition + new Vector3(0, openHeight, 0);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * openSpeed);
    }
}
