using UnityEngine;

public class UI_PanelManager : MonoBehaviour
{
    [Header("Place Panels here")]
    public GameObject[] panels;

    public void ShowPanel(int index)
    {
        if (index < 0 || index >= panels.Length) return;

        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == index);
        }
    }

    public void HideAllPanels()
    {
        foreach (GameObject panel in panels)
            panel.SetActive(false);
    }
}
