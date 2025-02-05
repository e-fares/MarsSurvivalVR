using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Vitesse de d�placement
    public float mouseSensitivity = 2f; // Sensibilit� de la souris
    private Rigidbody rb;
    private float cameraPitch = 0f; // Stocke l'angle vertical de la cam�ra
    public Transform cameraTransform; // R�f�rence � la cam�ra du joueur

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // �vite que le joueur bascule
        Cursor.lockState = CursorLockMode.Locked; // Verrouille le curseur au centre de l'�cran
    }

    void Update()
    {
        // D�placement du joueur
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;
        rb.velocity = moveDirection * moveSpeed + new Vector3(0, rb.velocity.y, 0);

        // Rotation avec la souris
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotation horizontale (autour du joueur)
        transform.Rotate(Vector3.up * mouseX);

        // Rotation verticale (de la cam�ra uniquement)
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f); // Limite l'angle vertical
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
}
