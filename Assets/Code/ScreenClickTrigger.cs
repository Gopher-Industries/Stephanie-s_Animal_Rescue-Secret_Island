using UnityEngine;

public class ScreenClickTrigger : MonoBehaviour
{
    [Header("Background Swap")]
    public SpriteRenderer bgRenderer;   // your BG’s SpriteRenderer
    public Sprite emptySprite;          // ControlCentre_Empty

    [Header("Dialogue UI")]
    public GameObject dialogueCanvas;   // Canvas (disabled at start)

    private bool clicked = false;
    void OnMouseDown()
    {
        if (clicked) return;
        clicked = true;

        // Swap background
        if (bgRenderer && emptySprite)
            bgRenderer.sprite = emptySprite;

        // Show UI & begin selection
        if (dialogueCanvas)
        {
            dialogueCanvas.SetActive(true);
            dialogueCanvas
              .GetComponent<DialogueDisplay>()
              .BeginSelection();
        }
    }
}
