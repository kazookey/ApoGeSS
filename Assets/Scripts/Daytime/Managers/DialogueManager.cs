using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup dialogueCanvasGroup;
    public CanvasGroup npcImageCanvasGroup;
    public Image npcImage;
    public TextMeshProUGUI dialogueText;
    public Button continueButton;

    [Header("Settings")]
    public float fadeDuration = 0.5f;
    public float typingSpeed = 20f;

    private int conversationStage = 0;
    private string currentSpiel;
    private Action onConversationFinished;

    private bool isTyping = false;
    private Tween typingTween;
    private string currentFullText; // Always holds the correct full text

    void Start()
    {
        // Hide at start
        if (dialogueCanvasGroup != null)
        {
            dialogueCanvasGroup.alpha = 0;
            dialogueCanvasGroup.blocksRaycasts = false;
        }

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
    }

    public void StartInteraction(string npcName, Sprite portrait, string spiel, Action onFinish)
    {
        // Store data
        currentSpiel = spiel;
        onConversationFinished = onFinish;

        if (npcImage != null)
            npcImage.sprite = portrait;

        npcImageCanvasGroup.alpha = 0;
        dialogueText.text = "";
        conversationStage = 0;

        // Fade in main panel
        dialogueCanvasGroup.blocksRaycasts = true;
        dialogueCanvasGroup.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            string intro = $"<b>{npcName}</b> would like to negotiate with you.";
            ShowText(intro);
        });
    }

    // Typing effect using counter tween
    void ShowText(string content)
    {
        isTyping = true;
        dialogueText.text = "";
        currentFullText = content; // Save the final text to fix skipping

        float duration = content.Length / typingSpeed;
        int currentChar = 0;

        typingTween = DOTween.To(() => currentChar, x => currentChar = x, content.Length, duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                dialogueText.text = currentFullText.Substring(0, currentChar);
            })
            .OnComplete(() =>
            {
                isTyping = false;
            });
    }

    void OnContinueClicked()
    {
        // CASE 1 — Skip typing
        if (isTyping)
        {
            typingTween.Kill();
            dialogueText.text = currentFullText; // Show the correct full text immediately
            isTyping = false;
            return;
        }

        // CASE 2 — Typing finished, move to next stage
        if (conversationStage == 0)
        {
            conversationStage = 1;

            // Fade in portrait
            npcImageCanvasGroup.DOFade(1f, fadeDuration);

            // Show the spiel
            ShowText(currentSpiel);
        }
        else
        {
            // End dialogue
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueCanvasGroup.blocksRaycasts = false;

        dialogueCanvasGroup.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            onConversationFinished?.Invoke();
        });
    }
}
