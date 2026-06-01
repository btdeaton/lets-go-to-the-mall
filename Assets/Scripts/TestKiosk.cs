using UnityEngine;

// Adding ': MonoBehaviour, IInteractable' means this script inherits from Unity, 
// AND agrees to the interaction contract.
public class TestKiosk : MonoBehaviour, IInteractable
{
    public string kioskName = "Information Desk";

    // We MUST implement this method because of the interface
    public void Interact()
    {
        Debug.Log($"You successfully interacted with the {kioskName}. Opening Management Menu...");
        // Later, you will trigger your UI or Management system here.
    }

    // We MUST implement this method because of the interface
    public string GetInteractPrompt()
    {
        return $"Press [E] to manage {kioskName}";
    }
}