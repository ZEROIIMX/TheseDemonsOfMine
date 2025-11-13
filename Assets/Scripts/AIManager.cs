using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(-100)]
public class AIManager : MonoBehaviour
{
    private static AIManager _instance;
    public static AIManager Instance
    {
        get
        {
            return _instance;
        }
        private set { _instance = value; }
    }

    public Transform Target;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (Target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Target = player.transform;
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
