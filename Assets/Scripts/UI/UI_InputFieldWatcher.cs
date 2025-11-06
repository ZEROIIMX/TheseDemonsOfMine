using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class InputFieldWatcher : MonoBehaviour
{
    public TMP_InputField inputField;
    public UnityEvent onTextNotEmpty;
    public UnityEvent onTextEmpty;
    bool wasEmpty = true;

    void Start()
    {
        inputField.onValueChanged.AddListener(OnValueChanged);
        OnValueChanged(inputField.text);
    }

    void OnValueChanged(string text)
    {
        bool isEmpty = string.IsNullOrWhiteSpace(text);
        if (wasEmpty && !isEmpty) onTextNotEmpty.Invoke();
        else if (!wasEmpty && isEmpty) onTextEmpty.Invoke();
        wasEmpty = isEmpty;
    }
}
