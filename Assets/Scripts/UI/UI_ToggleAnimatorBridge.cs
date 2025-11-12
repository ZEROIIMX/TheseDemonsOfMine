using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle), typeof(Animator))]
public class ToggleBuiltInTriggerBridge : MonoBehaviour
{
    public string onTrigger = "Selected"; // built-in name
    public string offTrigger = "Normal";  // built-in name

    Toggle toggle;
    Animator animator;
    Coroutine pending;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
        animator = GetComponent<Animator>();
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnEnable()
    {
        // Sync on enable (in case Animator was reset)
        ForceState(toggle.isOn, instant: true);
    }

    void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        // Let Unity fire its own hover/pressed triggers this frame,
        // then apply ours last so we win conflicts.
        if (pending != null) StopCoroutine(pending);
        pending = StartCoroutine(ApplyNextFrame(isOn));
    }

    IEnumerator ApplyNextFrame(bool isOn)
    {
        yield return null; // wait one frame
        ForceState(isOn, instant: false);
    }

    void ForceState(bool isOn, bool instant)
    {
        // Clear other built-in triggers that could override
        animator.ResetTrigger("Highlighted");
        animator.ResetTrigger("Pressed");
        animator.ResetTrigger("Selected");
        animator.ResetTrigger("Normal");
        animator.ResetTrigger("Disabled");

        // Fire the one we want based on isOn
        animator.SetTrigger(isOn ? onTrigger : offTrigger);

        // Optional: force base state immediately if your controller needs it
        // if (instant) animator.Play(isOn ? "Selected" : "Normal", 0, 0f);
    }
}
