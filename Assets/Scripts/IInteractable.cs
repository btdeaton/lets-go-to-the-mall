public interface IInteractable
{
    // What happens when the player presses 'E'
    void Interact();

    // The text to display on the screen when looking at the object (e.g., "Press E to Open Door")
    string GetInteractPrompt();
}