using UnityEngine;

public class DNASequenceFragment : MonoBehaviour
{
    [Header("Fragment Configuration")]
    public string fragmentName = "DNA Fragment";
    public string baseSequence = "ATCG";
    public int fragmentIndex = 0;
    
    [Header("Visual Components")]
    public DNABase[] basesInFragment;
    
    void Start()
    {
        InitializeFragment();
    }
    
    private void InitializeFragment()
    {
        // Auto-find DNA bases if not assigned
        if (basesInFragment == null || basesInFragment.Length == 0)
        {
            basesInFragment = GetComponentsInChildren<DNABase>();
        }
        
        // Set up bases according to sequence
        SetupBasesFromSequence();
    }
    
    private void SetupBasesFromSequence()
    {
        if (string.IsNullOrEmpty(baseSequence) || basesInFragment == null)
            return;
        
        for (int i = 0; i < basesInFragment.Length && i < baseSequence.Length; i++)
        {
            char baseChar = baseSequence[i];
            string baseType = CharToBaseType(baseChar);
            
            if (basesInFragment[i] != null)
            {
                basesInFragment[i].SetBaseType(baseType);
            }
        }
    }
    
    private string CharToBaseType(char baseChar)
    {
        switch (char.ToUpper(baseChar))
        {
            case 'A': return "A";
            case 'T': return "T";
            case 'G': return "G";
            case 'C': return "C";
            default: return "A";
        }
    }
    
    public string GetSequence()
    {
        return baseSequence;
    }
    
    public void SetSequence(string sequence)
    {
        baseSequence = sequence;
        SetupBasesFromSequence();
    }
    
    public DNABase[] GetBases()
    {
        return basesInFragment;
    }
    
    public bool IsComplete()
    {
        if (basesInFragment == null) return false;
        
        foreach (DNABase dnaBase in basesInFragment)
        {
            if (dnaBase != null && !dnaBase.IsPlaced())
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Reset all bases in this fragment to unplaced state
    /// </summary>
    public void ResetFragment()
    {
        if (basesInFragment == null) return;
        
        foreach (DNABase dnaBase in basesInFragment)
        {
            if (dnaBase != null)
            {
                dnaBase.ResetBase();
            }
        }
    }
}