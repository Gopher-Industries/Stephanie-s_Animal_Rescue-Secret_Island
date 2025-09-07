using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct Clue
{
    // One spread per element
    public string leftTitle;        // e.g., "Clues"
    public string[] clueLines;      // e.g., {"It hovers", "It's tiny", ...}

    public string rightTitle;       // e.g., "Multiple Choice Answers"
    public string[] options;        // up to 3 options shown as buttons
    public int correctIndex;        // 0..(options.Length-1) = correct; -1 = no quiz on this spread
}

public class ClueBookManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject cluePanel;
    public TMP_Text leftPageTitle;
    public TMP_Text leftPageContent;
    public TMP_Text rightPageTitle;
    public Button previousPageButton;
    public Button nextPageButton;
    public Button openClueBookButton;
    public Button closeClueBookButton;

    [Header("Right Page (3 answer buttons)")]
    public Button answerButton1;
    public Button answerButton2;
    public Button answerButton3;

    // Optional: assign these if you like; if left null we auto-find the TMP_Text inside each button.
    public TMP_Text answerText1;
    public TMP_Text answerText2;
    public TMP_Text answerText3;

    [Header("Clue Content")]
    public Clue[] clues;

    [Header("Behaviour")]
    public bool autoAdvanceOnCorrect = true;
    public float advanceDelay = 0.35f;

    private int currentPage = 0;
    private bool isOpen = false;
    private bool[] solved; // which pages already answered correctly

    void Awake()
    {
        if (cluePanel == null) cluePanel = gameObject;
        solved = (clues != null) ? new bool[clues.Length] : new bool[0];
    }

    void Start()
    {
        SetPanelVisible(false);
        UpdateCluePages();
    }

    // ----- Open / Close / Toggle -----
    public void OpenClueBook()
    {
        currentPage = 0;
        SetPanelVisible(true);
        UpdateCluePages();
    }

    public void CloseClueBook()
    {
        SetPanelVisible(false);
    }

    public void ToggleClueBook()
    {
        if (cluePanel == null) return;
        if (cluePanel.activeSelf) CloseClueBook();
        else OpenClueBook();
    }

    private void SetPanelVisible(bool visible)
    {
        isOpen = visible;

        if (cluePanel != null)
            cluePanel.SetActive(visible);

        if (openClueBookButton != null)
            openClueBookButton.gameObject.SetActive(!visible);

        if (closeClueBookButton != null)
            closeClueBookButton.gameObject.SetActive(visible);
    }

    // ----- Paging: one Clue = one spread -----
    public void NextPage()
    {
        if (clues == null || clues.Length == 0) return;
        if (currentPage < clues.Length - 1)
        {
            currentPage++;
            UpdateCluePages();
        }
    }

    public void PreviousPage()
    {
        if (clues == null || clues.Length == 0) return;
        if (currentPage > 0)
        {
            currentPage--;
            UpdateCluePages();
        }
    }

    // ----- Render current spread -----
    void UpdateCluePages()
    {
        if (clues == null || clues.Length == 0)
        {
            SetText(leftPageTitle, "");
            SetText(leftPageContent, "");
            SetText(rightPageTitle, "");
            if (previousPageButton) previousPageButton.gameObject.SetActive(false);
            if (nextPageButton) nextPageButton.gameObject.SetActive(false);
            SetAnswerButtonsActive(false, 0);
            return;
        }

        // Keep solved[] in sync with clues length (in case changed at runtime)
        if (solved == null || solved.Length != clues.Length)
            solved = new bool[clues.Length];

        var page = clues[Mathf.Clamp(currentPage, 0, clues.Length - 1)];

        // Left page
        SetText(leftPageTitle, string.IsNullOrEmpty(page.leftTitle) ? "Clues" : page.leftTitle);
        SetText(leftPageContent, BuildNumberedList(page.clueLines));

        // Right title
        SetText(rightPageTitle, string.IsNullOrEmpty(page.rightTitle) ? "Multiple Choice Answers" : page.rightTitle);

        // Quiz detection
        int optCount = (page.options != null) ? Mathf.Min(page.options.Length, 3) : 0;
        bool hasQuiz = optCount > 0 && page.correctIndex >= 0 && page.correctIndex < optCount;

        if (hasQuiz)
        {
            SetupAnswerButtons(page, optCount); // labels + click handlers
        }
        else
        {
            // No quiz: hide buttons
            SetAnswerButtonsActive(false, 0);
        }

        // Nav buttons
        if (previousPageButton) previousPageButton.gameObject.SetActive(currentPage > 0);

        if (nextPageButton)
        {
            nextPageButton.gameObject.SetActive(currentPage < clues.Length - 1);
            // If there is a quiz, lock Next until solved; otherwise allow Next
            nextPageButton.interactable = (!hasQuiz) || solved[currentPage];
        }
    }

    // ----- Build the 3 answer buttons -----
    void SetupAnswerButtons(Clue page, int count)
    {
        // Buttons must exist; texts can be auto-found.
        bool buttonsPresent = answerButton1 && answerButton2 && answerButton3;
        SetAnswerButtonsActive(buttonsPresent, count);
        if (!buttonsPresent) return;

        TMP_Text t1 = answerText1 ? answerText1 : answerButton1.GetComponentInChildren<TMP_Text>(true);
        TMP_Text t2 = answerText2 ? answerText2 : answerButton2.GetComponentInChildren<TMP_Text>(true);
        TMP_Text t3 = answerText3 ? answerText3 : answerButton3.GetComponentInChildren<TMP_Text>(true);

        SetText(t1, count > 0 ? page.options[0] : "");
        SetText(t2, count > 1 ? page.options[1] : "");
        SetText(t3, count > 2 ? page.options[2] : "");

        // Reset visuals / interactability
        ResetButtonVisual(answerButton1);
        ResetButtonVisual(answerButton2);
        ResetButtonVisual(answerButton3);

        bool alreadySolved = solved[currentPage];
        answerButton1.interactable = !alreadySolved && count > 0;
        answerButton2.interactable = !alreadySolved && count > 1;
        answerButton3.interactable = !alreadySolved && count > 2;

        // Clear old listeners
        answerButton1.onClick.RemoveAllListeners();
        answerButton2.onClick.RemoveAllListeners();
        answerButton3.onClick.RemoveAllListeners();

        // Wire new listeners (only for active buttons)
        if (count > 0) answerButton1.onClick.AddListener(() => OnAnswerClicked(0, page.correctIndex, answerButton1));
        if (count > 1) answerButton2.onClick.AddListener(() => OnAnswerClicked(1, page.correctIndex, answerButton2));
        if (count > 2) answerButton3.onClick.AddListener(() => OnAnswerClicked(2, page.correctIndex, answerButton3));
    }

    void OnAnswerClicked(int clickedIndex, int correctIndex, Button clickedButton)
    {
        bool isCorrect = (clickedIndex == correctIndex);

        // Simple color feedback (optional)
        var img = clickedButton ? clickedButton.GetComponent<Image>() : null;
        if (img != null) img.color = isCorrect ? new Color(0.75f, 1f, 0.75f) : new Color(1f, 0.75f, 0.75f);

        if (isCorrect)
        {
            solved[currentPage] = true;

            // Unlock Next
            if (nextPageButton)
            {
                nextPageButton.interactable = true;
                nextPageButton.gameObject.SetActive(currentPage < clues.Length - 1);
            }

            // Lock all answer buttons so the choice is final
            SetButtonsInteractable(false);

            // Auto-advance to the next spread if enabled
            if (autoAdvanceOnCorrect && currentPage < clues.Length - 1)
                Invoke(nameof(NextPage), advanceDelay);
        }
        else
        {
            // Optional: disable the clicked wrong button so they can't spam it
            // clickedButton.interactable = false;
        }
    }

    // ----- Utilities -----
    void SetButtonsInteractable(bool interactable)
    {
        if (answerButton1 && answerButton1.gameObject.activeSelf) answerButton1.interactable = interactable;
        if (answerButton2 && answerButton2.gameObject.activeSelf) answerButton2.interactable = interactable;
        if (answerButton3 && answerButton3.gameObject.activeSelf) answerButton3.interactable = interactable;
    }

    void ResetButtonVisual(Button b)
    {
        if (!b) return;
        var img = b.GetComponent<Image>();
        if (img != null) img.color = Color.white;
    }

    void SetAnswerButtonsActive(bool active, int count)
    {
        if (answerButton1) answerButton1.gameObject.SetActive(active && count > 0);
        if (answerButton2) answerButton2.gameObject.SetActive(active && count > 1);
        if (answerButton3) answerButton3.gameObject.SetActive(active && count > 2);
    }

    static void SetText(TMP_Text t, string v)
    {
        if (t) t.text = v;
    }

    static string BuildNumberedList(string[] lines)
    {
        if (lines == null || lines.Length == 0) return "";
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int num = 1;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            sb.Append(num++).Append(") ").Append(line.Trim()).Append('\n');
        }
        if (sb.Length > 0) sb.Length--; // trim last newline
        return sb.ToString();
    }
}
