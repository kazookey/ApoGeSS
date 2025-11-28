using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // IMPORTANT: Requires DOTween plugin
using System;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup dialogueCanvasGroup; // The parent panel
    public CanvasGroup npcImageCanvasGroup; // The character portrait
    public Image npcImage;                  // The actual sprite renderer
    public TextMeshProUGUI dialogueText;    // The text box
    public Button continueButton;           // The invisible full-screen button

    [Header("Settings")]
    public float fadeDuration = 0.5f;
    public float typingSpeed = 20f; // Characters per second

    // Internal State
    private int conversationStage = 0; 
    // 0 = Intro ("X wants to negotiate")
    // 1 = Spiel (Actual dialogue)
    
    private string currentSpiel;
    private Action onConversationFinished;
    private bool isTyping = false;
    private Tween typingTween; // Stores the animation so we can stop it

    void Start()
    {
        // Ensure everything is hidden at start
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
        // 1. Setup Data
        currentSpiel = spiel;
        onConversationFinished = onFinish;
        
        if (npcImage != null) npcImage.sprite = portrait;

        // Reset Visuals
        if (npcImageCanvasGroup != null) npcImageCanvasGroup.alpha = 0; // Hide face initially
        dialogueText.text = "";
        conversationStage = 0;

        // 2. Fade In Panel
        dialogueCanvasGroup.blocksRaycasts = true; // Enable clicking
        dialogueCanvasGroup.DOFade(1f, fadeDuration).OnComplete(() => 
        {
            // 3. Show Intro Text
            string introText = $"<b>{npcName}</b> would like to negotiate with you.";
            ShowText(introText);
        });
    }

    // Helper to animate text (Using "Option 1" - Manual Counter)
    void ShowText(string content)
    {
        isTyping = true;
        dialogueText.text = ""; // Clear
        
        float duration = content.Length / typingSpeed;
        int currentCharacterCount = 0;

        // "Generic" Tween: Animate a number from 0 to total characters
        typingTween = DOTween.To(() => currentCharacterCount, x => currentCharacterCount = x, content.Length, duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() => 
            {
                // Every frame, update the text to show X characters
                if (currentCharacterCount <= content.Length)
                    dialogueText.text = content.Substring(0, currentCharacterCount);
            })
            .OnComplete(() => isTyping = false);
    }

    void OnContinueClicked()
    {
        // Case A: Player clicks while text is still typing -> Skip to end
        if (isTyping)
        {
            typingTween.Kill(); // Stop the animation
            
            // Show the full text immediately
            if(conversationStage == 0)
                dialogueText.text = $"<b>{dialogueText.text.Replace("...", "")}</b> would like to negotiate with you."; // Just using the current text logic
            else 
                dialogueText.text = currentSpiel;
            
            // Fix: ensure text is fully visible based on stage logic
            if(conversationStage == 0 && !dialogueText.text.Contains("negotiate"))
                 dialogueText.text = "The Scavenger would like to negotiate with you.";

            isTyping = false;
            return;
        }

        // Case B: Text finished, move to next stage
        if (conversationStage == 0)
        {
            // Move to Stage 1: The Spiel
            conversationStage = 1;
            
            // Fade In NPC Face
            if (npcImageCanvasGroup != null) npcImageCanvasGroup.DOFade(1f, fadeDuration);
            
            // Start Typing Spiel
            ShowText(currentSpiel);
        }
        else if (conversationStage == 1)
        {
            // Conversation Over
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueCanvasGroup.blocksRaycasts = false;
        
        // Fade Out Dialogue Panel
        dialogueCanvasGroup.DOFade(0f, fadeDuration).OnComplete(() => 
        {
            // Trigger the callback (Opens Negotiation View)
            onConversationFinished?.Invoke();
        });
    }
}