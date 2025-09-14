using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct Clue
{
    public string leftTitle;
    public string[] clueLines;
    public string rightTitle;
    public string[] options;
    public int correctIndex;
    [TextArea] public string[] wrongFeedback;
    public Sprite revealSprite;
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

    public TMP_Text answerText1;
    public TMP_Text answerText2;
    public TMP_Text answerText3;

    [Header("Clue Content")]
    public Clue[] clues;

    [Header("Behaviour")]
    public bool autoAdvanceOnCorrect = true;
    public float advanceDelay = 0.35f;

    [Header("Feedback & Reveal")]
    public TMP_Text feedbackText;
    public Image revealImage;
    public bool hideRevealOnPageChange = true;

    private int currentPage = 0;
    private bool isOpen = false;
    private bool[] solved;

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

    public void OpenClueBook()
    {
        currentPage = 0;
        SetPanelVisible(true);
        if (clues != null) solved = new bool[clues.Length];
        if (revealImage) revealImage.gameObject.SetActive(false);
        if (feedbackText) { feedbackText.text = ""; feedbackText.gameObject.SetActive(true); }
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
        if (cluePanel != null) cluePanel.SetActive(visible);
        if (openClueBookButton != null) openClueBookButton.gameObject.SetActive(!visible);
        if (closeClueBookButton != null) closeClueBookButton.gameObject.SetActive(visible);
    }

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
            if (feedbackText) { feedbackText.text = ""; feedbackText.gameObject.SetActive(true); }
            if (revealImage) revealImage.gameObject.SetActive(false);
            return;
        }

        if (solved == null || solved.Length != clues.Length)
            solved = new bool[clues.Length];

        var page = clues[Mathf.Clamp(currentPage, 0, clues.Length - 1)];

        if (leftPageTitle) leftPageTitle.gameObject.SetActive(true);
        if (leftPageContent) leftPageContent.gameObject.SetActive(true);
        if (rightPageTitle) rightPageTitle.gameObject.SetActive(true);

        SetText(leftPageTitle, string.IsNullOrEmpty(page.leftTitle) ? "Clues" : page.leftTitle);
        SetText(leftPageContent, BuildNumberedList(page.clueLines));
        SetText(rightPageTitle, string.IsNullOrEmpty(page.rightTitle) ? "Multiple Choice Answers" : page.rightTitle);

        if (feedbackText) { feedbackText.text = ""; feedbackText.gameObject.SetActive(true); }
        if (revealImage && hideRevealOnPageChange) revealImage.gameObject.SetActive(false);

        int optCount = (page.options != null) ? Mathf.Min(page.options.Length, 3) : 0;
        bool hasQuiz = optCount > 0 && page.correctIndex >= 0 && page.correctIndex < optCount;

        if (hasQuiz) SetupAnswerButtons(page, optCount);
        else SetAnswerButtonsActive(false, 0);

        if (previousPageButton) previousPageButton.gameObject.SetActive(currentPage > 0);
        if (nextPageButton)
        {
            nextPageButton.gameObject.SetActive(currentPage < clues.Length - 1);
            nextPageButton.interactable = (!hasQuiz) || solved[currentPage];
        }
    }

    void SetupAnswerButtons(Clue page, int count)
    {
        bool buttonsPresent = answerButton1 && answerButton2 && answerButton3;
        SetAnswerButtonsActive(buttonsPresent, count);
        if (!buttonsPresent) return;

        TMP_Text t1 = answerText1 ? answerText1 : answerButton1.GetComponentInChildren<TMP_Text>(true);
        TMP_Text t2 = answerText2 ? answerText2 : answerButton2.GetComponentInChildren<TMP_Text>(true);
        TMP_Text t3 = answerText3 ? answerText3 : answerButton3.GetComponentInChildren<TMP_Text>(true);

        SetText(t1, count > 0 ? page.options[0] : "");
        SetText(t2, count > 1 ? page.options[1] : "");
        SetText(t3, count > 2 ? page.options[2] : "");

        ResetButtonVisual(answerButton1);
        ResetButtonVisual(answerButton2);
        ResetButtonVisual(answerButton3);

        bool alreadySolved = solved[currentPage];
        answerButton1.interactable = !alreadySolved && count > 0;
        answerButton2.interactable = !alreadySolved && count > 1;
        answerButton3.interactable = !alreadySolved && count > 2;

        answerButton1.onClick.RemoveAllListeners();
        answerButton2.onClick.RemoveAllListeners();
        answerButton3.onClick.RemoveAllListeners();

        if (count > 0) answerButton1.onClick.AddListener(() => OnAnswerClicked(0, page.correctIndex, answerButton1));
        if (count > 1) answerButton2.onClick.AddListener(() => OnAnswerClicked(1, page.correctIndex, answerButton2));
        if (count > 2) answerButton3.onClick.AddListener(() => OnAnswerClicked(2, page.correctIndex, answerButton3));
    }

    void OnAnswerClicked(int clickedIndex, int correctIndex, Button clickedButton)
    {
        bool isCorrect = (clickedIndex == correctIndex);

        var img = clickedButton ? clickedButton.GetComponent<Image>() : null;
        if (img != null)
            img.color = isCorrect ? new Color(0.75f, 1f, 0.75f)
                                  : new Color(1f, 0.75f, 0.75f);

        var page = (clues != null && currentPage >= 0 && currentPage < clues.Length) ? clues[currentPage] : default;

        if (isCorrect)
        {
            solved[currentPage] = true;

            if (feedbackText) { feedbackText.text = ""; feedbackText.gameObject.SetActive(false); }
            if (leftPageTitle) leftPageTitle.gameObject.SetActive(false);
            if (leftPageContent) leftPageContent.gameObject.SetActive(false);
            if (rightPageTitle) rightPageTitle.gameObject.SetActive(false);
            SetAnswerButtonsActive(false, 0);

            if (revealImage && page.revealSprite != null)
            {
                revealImage.sprite = page.revealSprite;
                revealImage.type = Image.Type.Simple;
                revealImage.preserveAspect = true;
                revealImage.gameObject.SetActive(true);
            }

            if (nextPageButton)
            {
                nextPageButton.interactable = true;
                nextPageButton.gameObject.SetActive(currentPage < clues.Length - 1);
            }

            SetButtonsInteractable(false);

            if (autoAdvanceOnCorrect && currentPage < clues.Length - 1)
                Invoke(nameof(NextPage), advanceDelay);
        }
        else
        {
            string reason = null;
            if (page.wrongFeedback != null &&
                clickedIndex >= 0 &&
                clickedIndex < page.wrongFeedback.Length &&
                !string.IsNullOrWhiteSpace(page.wrongFeedback[clickedIndex]))
            {
                reason = page.wrongFeedback[clickedIndex];
            }
            else
            {
                string optText = (page.options != null && clickedIndex < page.options.Length) ? page.options[clickedIndex] : "That";
                reason = "Oops! " + optText + " isn’t correct.";
            }

            if (feedbackText) { feedbackText.gameObject.SetActive(true); feedbackText.text = reason; }
        }
    }

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

    static void SetText(TMP_Text t, string v) { if (t) t.text = v; }

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
        if (sb.Length > 0) sb.Length--;
        return sb.ToString();
    }
}
