using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabsManager : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public Toggle toggle;        // Left-side toggle
        public GameObject panel;     // Right-side content panel
        [Tooltip("Optional: first UI element to focus when this panel opens")]
        public GameObject firstFocus;
    }

    [Header("Assign your tabs in order")]
    public List<Tab> tabs = new();
    public ToggleGroup toggleGroup;
    public int defaultTabIndex = 0;

    void Awake()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            var idx = i;
            if (tabs[i].toggle)
            {
                tabs[i].toggle.group = toggleGroup;
                tabs[i].toggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn) ShowTab(idx, true);
                });
            }
        }
    }

    void Start()
    {
        if (tabs.Count == 0) return;
        defaultTabIndex = Mathf.Clamp(defaultTabIndex, 0, tabs.Count - 1);

        // Initialize tab state
        for (int i = 0; i < tabs.Count; i++)
            if (tabs[i].toggle) tabs[i].toggle.isOn = (i == defaultTabIndex);

        ShowTab(defaultTabIndex, false);
    }

    public void ShowTab(int index, bool moveFocus)
    {
        // Enable only the selected panel
        for (int i = 0; i < tabs.Count; i++)
            if (tabs[i].panel) tabs[i].panel.SetActive(i == index);

        if (moveFocus)
        {
            // Move focus to first UI element in the active panel
            var target = tabs[index].firstFocus != null
                ? tabs[index].firstFocus
                : tabs[index].toggle != null ? tabs[index].toggle.gameObject : null;

            if (target) EventSystem.current?.SetSelectedGameObject(target);
        }
    }
}
