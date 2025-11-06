using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonTMPHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI textTarget;
    public float minScale = 0.9f;
    public float maxScale = 1.3f;
    public float speed = 4f;

    TMP_MeshInfo[] cachedMeshInfo;
    float[] phasePerChar;
    bool hovering;
    Coroutine animCo;

    void Awake()
    {
        if (textTarget == null)
            textTarget = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textTarget == null) return;
        StartHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (textTarget == null) return;
        StopHover();
    }

    void StartHover()
    {
        hovering = true;
        textTarget.ForceMeshUpdate();
        cachedMeshInfo = textTarget.textInfo.CopyMeshInfoVertexData();
        EnsurePhaseArray();
        if (animCo == null) animCo = StartCoroutine(Animate());
    }

    void StopHover()
    {
        hovering = false;
        if (animCo != null) { StopCoroutine(animCo); animCo = null; }
        RestoreCachedMesh();
    }

    System.Collections.IEnumerator Animate()
    {
        while (hovering)
        {
            textTarget.ForceMeshUpdate();
            var textInfo = textTarget.textInfo;
            if (cachedMeshInfo == null || cachedMeshInfo.Length != textInfo.meshInfo.Length)
                cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
            EnsurePhaseArray();

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int matIndex = charInfo.materialReferenceIndex;
                int vertIndex = charInfo.vertexIndex;

                Vector3[] sourceVerts = cachedMeshInfo[matIndex].vertices;
                Vector3[] destVerts = textInfo.meshInfo[matIndex].vertices;

                Vector3 c = (sourceVerts[vertIndex + 0] + sourceVerts[vertIndex + 2]) * 0.5f;

                float t = Time.time * speed + phasePerChar[i];
                float s = Mathf.Lerp(minScale, maxScale, 0.5f + 0.5f * Mathf.Sin(t));

                Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(s, s, 1f));

                for (int j = 0; j < 4; j++)
                {
                    Vector3 v = sourceVerts[vertIndex + j] - c;
                    destVerts[vertIndex + j] = m.MultiplyPoint3x4(v) + c;
                }
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                textTarget.UpdateGeometry(meshInfo.mesh, i);
            }

            yield return null;
        }
    }

    void EnsurePhaseArray()
    {
        int count = textTarget.textInfo.characterCount;
        if (phasePerChar == null || phasePerChar.Length != count)
        {
            phasePerChar = new float[count];
            for (int i = 0; i < count; i++) phasePerChar[i] = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    void RestoreCachedMesh()
    {
        textTarget.ForceMeshUpdate();
        var textInfo = textTarget.textInfo;
        if (cachedMeshInfo == null) cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var src = cachedMeshInfo[i].vertices;
            var dst = textInfo.meshInfo[i].vertices;
            System.Array.Copy(src, dst, src.Length);
            textInfo.meshInfo[i].mesh.vertices = dst;
            textTarget.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
