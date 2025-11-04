using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class RuntimeTransformSaver : MonoBehaviour
{
    [System.Serializable]
    public class SavedTransform
    {
        public string path; // Full scene path
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    [System.Serializable]
    public class TransformSaveData
    {
        public List<SavedTransform> transforms = new();
    }

    private string savePath => Path.Combine(Application.dataPath, "SavedTransforms.json");

    public void SaveCurrentTransforms()
    {
        TransformSaveData data = new();
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            string fullPath = GetFullPath(child);
            data.transforms.Add(new SavedTransform
            {
                path = fullPath,
                position = child.position,
                rotation = child.rotation,
                scale = child.localScale
            });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Saved {data.transforms.Count} transforms to: {savePath}");
    }

    public void LoadSavedTransforms()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No saved transform file found.");
            return;
        }

        string json = File.ReadAllText(savePath);
        TransformSaveData data = JsonUtility.FromJson<TransformSaveData>(json);

        foreach (SavedTransform saved in data.transforms)
        {
            Transform target = FindTransformByPath(saved.path);
            if (target != null)
            {
                target.position = saved.position;
                target.rotation = saved.rotation;
                target.localScale = saved.scale;
                Debug.Log($"Restored: {saved.path}");
            }
            else
            {
                Debug.LogWarning($"Could not find: {saved.path}");
            }
        }
    }

    private string GetFullPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }

    private Transform FindTransformByPath(string path)
    {
        string[] parts = path.Split('/');
        Transform current = null;

        foreach (string part in parts)
        {
            if (current == null)
            {
                current = GameObject.Find(part)?.transform;
            }
            else
            {
                current = current.Find(part);
            }

            if (current == null) break;
        }

        return current;
    }
}
