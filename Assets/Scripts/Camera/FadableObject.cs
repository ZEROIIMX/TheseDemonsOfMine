using UnityEngine;

public class FadableObject : MonoBehaviour
{
    public Material fadeMaterial;
    public Material opaqueMaterial;
    [HideInInspector] public Material runtimeFadeInstance;
}
