using System.Collections.Generic;
using System.Linq;

public class InteractManager : StaticInstance<InteractManager> {

    private List<Interactable> interactablesWithinRange = new();

    public void TryAddWithinRange(Interactable interactable) {
        if (!interactablesWithinRange.Contains(interactable)) {
            AddWithinRange(interactable);
        }
    }

    public void AddWithinRange(Interactable interactable) {
        interactablesWithinRange.Add(interactable);
        UpdateWhichCanInteract();
    }

    public void TryRemoveWithinRange(Interactable interactable) {
        if (interactablesWithinRange.Contains(interactable)) {
            RemoveWithinRange(interactable);
        }
    }

    public void RemoveWithinRange(Interactable interactable) {
        interactablesWithinRange.Remove(interactable);
        if (interactable.CanInteract) {
            interactable.SetCantInteract();
        }
        UpdateWhichCanInteract();
    }

    private void UpdateWhichCanInteract() {

        if (interactablesWithinRange.Count == 0) {
            return;
        }

        foreach (var interactable in interactablesWithinRange) {
            if (interactable.CanInteract) {
                interactable.SetCantInteract();
            }
        }
        
        interactablesWithinRange.Last().SetCanInteract();
    }
}
