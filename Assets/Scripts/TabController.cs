using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;

    private Color inactiveColor;
    private Color activeColor;

    void Start()
    {
        // Parse your hex (#640D80)
        if (ColorUtility.TryParseHtmlString("#640D80", out Color baseColor))
        {
            inactiveColor = baseColor;

            // Make a lighter version (adjust brightness by ~20%)
            activeColor = new Color(
                Mathf.Clamp01(baseColor.r * 1.2f),
                Mathf.Clamp01(baseColor.g * 1.2f),
                Mathf.Clamp01(baseColor.b * 1.2f),
                baseColor.a
            );
        }
        else
        {
            // Fallback if hex fails
            inactiveColor = Color.grey;
            activeColor = Color.white;
        }

        ActivateTab(0);
    }

    public void ActivateTab(int tabNo)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = inactiveColor;
        }
        pages[tabNo].SetActive(true);
        tabImages[tabNo].color = activeColor;
    }
}
