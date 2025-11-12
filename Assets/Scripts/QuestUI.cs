using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public Transform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;

    // public Quest testQuest;
    // public int testQuestAmount;
    // private List<QuestProgress> testQuests = new();

    // Start is called before the first frame update
    void Start()
    {
        // for(int i = 0; i < testQuestAmount; i++)
        // {
        //     testQuests.Add(new QuestProgress(testQuest));
        // }

        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        // Check if QuestController.Instance is available
        if (QuestController.Instance == null)
        {
            Debug.LogWarning("QuestUI: QuestController.Instance is null, cannot update quest UI");
            return;
        }

        // Check if required UI components are available
        if (questListContent == null)
        {
            Debug.LogError("QuestUI: questListContent is null, cannot update quest UI");
            return;
        }

        if (questEntryPrefab == null)
        {
            Debug.LogError("QuestUI: questEntryPrefab is null, cannot update quest UI");
            return;
        }

        if (objectiveTextPrefab == null)
        {
            Debug.LogError("QuestUI: objectiveTextPrefab is null, cannot update quest UI");
            return;
        }

        //Destroy existing quest entries
        foreach(Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }

        //Build quest entries
        foreach(var quest in QuestController.Instance.activeQuests)
        {
            if (quest?.quest == null) continue;

            GameObject entry = Instantiate(questEntryPrefab, questListContent);
            TMP_Text questNameText = entry.transform.Find("QuestNameText").GetComponent<TMP_Text>();
            Transform objectiveList = entry.transform.Find("ObjectiveList");

            if (questNameText != null)
            {
                questNameText.text = quest.quest.questName;
            }

            if (objectiveList != null && quest.objectives != null)
            {
                foreach(var objective in quest.objectives)
                {
                    if (objective == null) continue;

                    GameObject objTextGO = Instantiate(objectiveTextPrefab, objectiveList);
                    TMP_Text objText = objTextGO.GetComponent<TMP_Text>();
                    if (objText != null)
                    {
                        objText.text = $"{objective.description} ({objective.currentAmount}/{objective.requiredAmount})"; //Collect 5 Potions (0/5)
                    }
                }
            }
        }
    }

}

