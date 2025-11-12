using UnityEngine;

[System.Serializable]
public class FragmentProperties
{
    [Header("Fragment Configuration")]
    public string fragmentName = "DNA Fragment";
    public string baseSequence = "ATCG";
    public Color fragmentColor = Color.white;
    public bool isCorrect = false;
    
    [Header("Visual Properties")]
    public Sprite fragmentSprite;
    public float mutationChance = 0.1f;
    
    public FragmentProperties()
    {
        // Default constructor
    }
    
    public FragmentProperties(string name, string sequence, bool correct)
    {
        fragmentName = name;
        baseSequence = sequence;
        isCorrect = correct;
    }
}