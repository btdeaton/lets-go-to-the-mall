using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 3f;
    public LayerMask interactableLayer; // Keeps the raycast cheap by ignoring walls/floors

    private Camera playerCamera;
    private IInteractable currentTarget;

    void Start()
    {
        playerCamera = GetComponent<Camera>();
    }

    void Update()
    {
        HandleRaycast();
        HandleInput();
    }

    private void HandleRaycast()
    {
        // Shoot a ray from the center of the screen forward
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // If the ray hits something on the 'Interactable' layer within range
        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            // Check if the object we hit has a script implementing IInteractable
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentTarget = interactable;

                // TODO for later: Send currentTarget.GetInteractPrompt() to your UI Manager
                // Debug.Log(currentTarget.GetInteractPrompt()); 
                return;
            }
        }

        // If we hit nothing or looked away, clear the target
        currentTarget = null;
    }

    private void HandleInput()
    {
        // If we are looking at a valid target AND we press the 'E' key this specific frame
        if (currentTarget != null && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentTarget.Interact();
        }
    }
}