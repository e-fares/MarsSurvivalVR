using UnityEngine;

public class MiniMapPlaneController : MonoBehaviour
{
    public Camera mainCamera;

    void Update()
    {
        // Orienter le plan en direction de la camera
        transform.LookAt(mainCamera.transform);

        // corriger la rotation du plan
        transform.rotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);
    }
}
