using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueDisplay : MonoBehaviour
{
    // ── Data Structures ─────────────────────────────────────────────────────────

    [System.Serializable]
    public struct AnimalData
    {
        public string name;
        [TextArea] public string funFact;
        [TextArea] public string tip;
    }

    private class Line
    {
        public bool isSteph;       // true = Stéphanie, false = Ketch
        public string text;
        public Sprite stephPose;   // Stéphanie’s world‐pose
        public bool grabHat;       // does Steph grab her hat here?
    }

    // ── Inspector Fields ────────────────────────────────────────────────────────

    [Header("1) Animal Info (4 items)")]
    public AnimalData[] animals = new AnimalData[4];

    [Header("2) Stéphanie’s Poses")]
    public Sprite poseEntry;     // stephie_walking_towards
    public Sprite poseQuestion;  // stephie_listening
    public Sprite poseLeave;     // stephie_walking_away

    [Header("3) Selection UI")]
    public GameObject selectionPanel;
    public TMP_Dropdown dropdown;
    public Button confirmButton;

    [Header("4) Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI NameText;     // "Stephanie" or "Ketch"
    public Image KetchPortraitUI;        // only Ketch’s portrait
    public TextMeshProUGUI dialogueText; // the typewriter text field

    [Header("5) World References")]
    public SpriteRenderer stephRenderer; // Stéphanie in the scene
    public Transform hatAnchor;          // Stéphanie→HatAnchor
    public GameObject hatObject;         // the desk hat

    // ── Private State ─────────────────────────────────────────────────────────

    private List<Line> lines;
    private int index = 0;

    // ── Public API ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Called by ScreenClickTrigger after swapping the BG & enabling the Canvas.
    /// </summary>
    public void BeginSelection()
    {
        // Populate the dropdown with animal names
        dropdown.ClearOptions();
        List<string> opts = new List<string>();
        foreach (var a in animals) opts.Add(a.name);
        dropdown.AddOptions(opts);

        // Show the selection UI
        selectionPanel.SetActive(true);
        dialoguePanel.SetActive(false);

        // Hook up the Go! button
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnAnimalPicked);
    }

    // ── Handlers & Coroutines ──────────────────────────────────────────────────

    private void OnAnimalPicked()
    {
        // Read the chosen animal
        AnimalData data = animals[dropdown.value];

        // Build a 6‐line back-and-forth:
        lines = new List<Line> {
            // 1) Ketch intro
            new Line {
                isSteph  = false,
                text     = $"Stephanie, we’ve got a {data.name} that needs rescue.",
                stephPose= null,
                grabHat  = false
            },
            // 2) Steph question
            new Line {
                isSteph  = true,
                text     = $"What can you tell me about this {data.name}?",
                stephPose= poseQuestion,
                grabHat  = false
            },
            // 3) Ketch fun fact
            new Line {
                isSteph  = false,
                text     = $"Fun Fact: {data.funFact}",
                stephPose= null,
                grabHat  = false
            },
            // 4) Steph question on help
            new Line {
                isSteph  = true,
                text     = $"How can we help the {data.name}?",
                stephPose= poseQuestion,
                grabHat  = false
            },
            // 5) Ketch tip
            new Line {
                isSteph  = false,
                text     = $"Conservation Tip: {data.tip}",
                stephPose= null,
                grabHat  = false
            },
            // 6) Steph final + hat grab
            new Line {
                isSteph  = true,
                text     = $"Understood—let’s go save that {data.name}!",
                stephPose= poseLeave,
                grabHat  = true
            }
        };

        // Switch UI panels
        selectionPanel.SetActive(false);
        dialoguePanel.SetActive(true);

        // Start the dialogue coroutine
        index = 0;
        StartCoroutine(PlayDialogue());
    }

    private IEnumerator PlayDialogue()
    {
        // Apply styling & portrait for this line
        Line L = lines[index];
        ApplyLine(L);

        // Typewriter effect
        dialogueText.text = "";
        foreach (char c in L.text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.04f);
        }

        // Wait for the user to click
        while (!Input.GetMouseButtonDown(0))
            yield return null;

        // Next line…
        index++;
        if (index < lines.Count)
            StartCoroutine(PlayDialogue());
    }

    // ── Helper to style each line ───────────────────────────────────────────────

    private void ApplyLine(Line L)
    {
        // 1) Nameplate
        NameText.text = L.isSteph ? "Stephanie" : "Ketch";

        // 2) Portrait: show only when Ketch speaks
        KetchPortraitUI.gameObject.SetActive(!L.isSteph);

        // 3) Stéphanie’s world‐pose when she speaks
        if (L.isSteph && L.stephPose != null)
            stephRenderer.sprite = L.stephPose;

        // 4) Hat grab at the very end
        if (L.grabHat && hatObject && hatAnchor)
        {
            hatObject.transform.SetParent(hatAnchor, false);
            hatObject.transform.localPosition = Vector3.zero;
            hatObject.transform.localRotation = Quaternion.identity;
        }
    }
}
