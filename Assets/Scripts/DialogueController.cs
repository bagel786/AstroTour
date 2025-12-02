using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static DialogueController Instance { get; private set; }
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public Image portraitImage;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;
    
    [Header("Choice Positioning")]
    [Tooltip("Base Y position offset from dialogue box for 1 choice")]
    public float baseYOffset = 50f;
    [Tooltip("Additional Y offset per extra choice")]
    public float offsetPerChoice = 40f;
    
    private RectTransform choiceContainerRect;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        choiceContainerRect = choiceContainer.GetComponent<RectTransform>();
    }
    public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show);
    }
    public void SetNPCInfo(string npcName, Sprite portrait)
    {
        nameText.text = npcName;
        portraitImage.sprite = portrait;
        
        // Make portrait image transparent if no portrait is provided
        if (portrait == null)
        {
            // Set alpha to 0 (fully transparent)
            Color imageColor = portraitImage.color;
            imageColor.a = 0f;
            portraitImage.color = imageColor;
        }
        else
        {
            // Ensure portrait is visible when sprite is provided
            Color imageColor = portraitImage.color;
            imageColor.a = 1f;
            portraitImage.color = imageColor;
        }
    }
    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }
    public void ClearChoices()
    {
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }

    }
    public void CreateChoiceButton(string choiceText, UnityEngine.Events.UnityAction onClick)
    {
        GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText;
        choiceButton.GetComponent<Button>().onClick.AddListener(onClick);
    }
    
    public void AdjustChoiceContainerPosition(int numberOfChoices)
    {
        if (choiceContainerRect == null) return;
        
        // Calculate offset based on number of choices
        // Fewer choices = closer to dialogue box
        float yOffset = baseYOffset + (offsetPerChoice * (numberOfChoices - 1));
        
        // Update the anchored position
        Vector2 currentPos = choiceContainerRect.anchoredPosition;
        choiceContainerRect.anchoredPosition = new Vector2(currentPos.x, yOffset);
    }

}
