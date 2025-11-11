using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    public Renderer targetRenderer;
    public Color emissionColor = Color.white;
    [Range(0f, 10f)] public float emissionIntensity = 1f;

    private MaterialPropertyBlock propBlock;

    void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();
    }

    void Start()
    {
        UpdateEmission();
    }

    public void HealthUpdate(float healthPercent)
    {
        emissionIntensity = (healthPercent / 30);
        UpdateEmission();
    }

    public void Flash(float healthpercent)
    {
        emissionIntensity = (healthpercent / 300);
        UpdateEmission();
        StartCoroutine(ResetEmission(healthpercent));
    }

    private IEnumerator ResetEmission(float healthpercent)
    {
        yield return new WaitForSeconds(0.2f);
        emissionIntensity = (healthpercent/ 30);
        UpdateEmission();
    }

    private void UpdateEmission()
    {
        targetRenderer.GetPropertyBlock(propBlock);
        Color finalColor = emissionColor * Mathf.LinearToGammaSpace(emissionIntensity);
        propBlock.SetColor("_EmissionColor", finalColor);
        targetRenderer.SetPropertyBlock(propBlock);
    }
}
