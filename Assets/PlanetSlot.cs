using UnityEngine;

public class PlanetSlot : MonoBehaviour
{
    public string planetName; // Assign in Inspector

    private void Start()
    {
        gameObject.name = planetName + "_Slot"; // Name the slot dynamically
    }
}
