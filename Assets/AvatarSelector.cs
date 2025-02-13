using UnityEngine;
using UnityEngine.UI;

public class AvatarSelector : MonoBehaviour
{
    public GameObject[] avatars; // Assign all avatar GameObjects in the Inspector
    public GameObject[] avatarsCast; // Assign all avatar GameObjects in the Inspector

    private int currentIndex = 0; // Keeps track of the selected avatar index

    void Start()
    {
        SetActiveAvatar(currentIndex);
    }

    public void NextAvatar()
    {
        currentIndex = (currentIndex + 1) % avatars.Length; // Loop to first when at the end
        SetActiveAvatar(currentIndex);
    }

    public void PreviousAvatar()
    {
        currentIndex = (currentIndex - 1 + avatars.Length) % avatars.Length; // Loop to last when at the start
        SetActiveAvatar(currentIndex);
    }

    private void SetActiveAvatar(int avatarIndex)
    {
        // Disable all avatars
        foreach (GameObject avatar in avatars)
        {
            avatar.SetActive(false);
        }
        foreach (GameObject avatarcast in avatarsCast)
        {
            avatarcast.SetActive(false);
        }

        // Enable the selected avatar
        if (avatars.Length > 0)
        {
            avatars[avatarIndex].SetActive(true);
            avatarsCast[avatarIndex].SetActive(true);

        }
    }
}
