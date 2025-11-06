using UnityEngine;
using System.Collections.Generic;

public class ObstructionFader : MonoBehaviour
{
    public Transform player;
    public LayerMask fadeLayerA;
    public LayerMask fadeLayerB;
    public float fadeSpeed = 2f;
    public float targetFadeAmount = 0.3f;

    private class FadeData
    {
        public FadableObject fadable;
        public Renderer renderer;
        public bool isObstructing;
        public bool usingFadeMaterial;
    }

    private Dictionary<Renderer, FadeData> fadedObjects = new();

    void Update()
    {
        if (player == null) return;

        // 🧹 Clean up destroyed or invalid objects
        List<Renderer> cleanupList = new();
        foreach (var kvp in fadedObjects)
        {
            var data = kvp.Value;
            if (kvp.Key == null || data == null || data.fadable == null || data.renderer == null)
                cleanupList.Add(kvp.Key);
        }
        foreach (var rend in cleanupList)
            fadedObjects.Remove(rend);

        // Reset obstruction flags
        foreach (var data in fadedObjects.Values)
        {
            if (data == null || data.fadable == null || data.renderer == null) continue;
            data.isObstructing = false;
        }

        // Detect obstructions
        Vector3 direction = player.position - transform.position;
        float distance = Vector3.Distance(transform.position, player.position);
        int combinedMask = fadeLayerA | fadeLayerB;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, combinedMask);

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            FadableObject fadable = hit.collider.GetComponent<FadableObject>();
            if (rend == null || fadable == null) continue;

            if (!fadedObjects.ContainsKey(rend))
            {
                if (fadable.fadeMaterial == null) continue;

                fadable.runtimeFadeInstance = new Material(fadable.fadeMaterial);
                rend.material = fadable.runtimeFadeInstance;

                fadedObjects[rend] = new FadeData
                {
                    fadable = fadable,
                    renderer = rend,
                    isObstructing = true,
                    usingFadeMaterial = true
                };
            }

            if (!fadedObjects.TryGetValue(rend, out var data) || data == null || data.renderer == null || data.fadable == null) continue;

            data.isObstructing = true;

            Material mat = data.renderer.material;
            if (data.usingFadeMaterial && mat != null && mat.HasProperty("_BaseColor"))
            {
                Color current = mat.GetColor("_BaseColor");
                float fadedAlpha = Mathf.MoveTowards(current.a, targetFadeAmount, Time.deltaTime * fadeSpeed);
                mat.SetColor("_BaseColor", new Color(current.r, current.g, current.b, fadedAlpha));
            }
        }

        // Restore non-obstructing objects
        List<Renderer> toRemove = new();
        foreach (var kvp in fadedObjects)
        {
            var data = kvp.Value;
            if (data == null || data.renderer == null || data.fadable == null) continue;
            if (data.isObstructing) continue;

            Material mat = data.renderer.material;
            if (data.usingFadeMaterial && mat != null && mat.HasProperty("_BaseColor"))
            {
                Color current = mat.GetColor("_BaseColor");
                float restoredAlpha = Mathf.MoveTowards(current.a, 1f, Time.deltaTime * fadeSpeed);
                mat.SetColor("_BaseColor", new Color(current.r, current.g, current.b, restoredAlpha));

                if (restoredAlpha >= 1f && data.fadable.opaqueMaterial != null)
                {
                    data.renderer.material = data.fadable.opaqueMaterial;
                    data.usingFadeMaterial = false;
                    toRemove.Add(data.renderer);
                }
            }
        }

        foreach (var rend in toRemove)
            fadedObjects.Remove(rend);
    }
}
