using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public float openHeight = 3f;
    public float openSpeed = 2f;
    public AudioClip openSound;
    public AudioClip deniedSound;
    public VRDoorHandle vrDoorHandle;
    public VRDoorHandle vrDoorHandle2;
    public VRDoorHandle vrDoorHandle3;
    public VRDoorHandle vrDoorHandle4;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private AudioSource audioSource;

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition + new Vector3(0, openHeight, 0);
        audioSource = GetComponent<AudioSource>();
    }

    public void OpenDoor()
    {
        if (vrDoorHandle.getCount() == 2 || vrDoorHandle2.getCount() == 2 || vrDoorHandle3.getCount() == 2 || vrDoorHandle4.getCount() == 2)
        {
            StopAllCoroutines();
            StartCoroutine(MoveDoor(targetPosition));
        }
      
    }

    private System.Collections.IEnumerator MoveDoor(Vector3 target)
    {
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * openSpeed);
            yield return null;
        }
        transform.position = target;
    }
}
