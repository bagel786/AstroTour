using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public string mapBoundary; //The boundary name for the map
    public List<InventorySaveData> inventorySaveData;
    public List<InventorySaveData> hotbarSaveData;
    public List<BoxSaveData> boxSaveData;
    public List<TerminalSaveData> terminalSaveData;
    public List<QuestProgress> questProgressData;
    public List<string> handInQuestIDS;
    public List<string> givenDialogueRewardIDs;
}

[System.Serializable]
public class BoxSaveData
{
    public string boxID;
    public bool isOpened;
}

[System.Serializable]
public class TerminalSaveData
{
    public string terminalID;
    public bool hasAccessed;
    public bool hasInteracted;
}