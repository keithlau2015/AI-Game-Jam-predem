using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T>: MonoBehaviour where T : Component
{
    protected static T instance;
    private static bool exitApplication = false;
    private static readonly object locker = new object();
    public static T singleton
    {
        get
        {
            lock (locker)
            {
                if (exitApplication)
                    return null;

                if (instance != null)
                    return instance;                

                var objs = FindObjectsOfType(typeof(T)) as T[];
                if (objs.Length > 1)
                    Debug.LogError($"There is more than one {typeof(T).Name} in the scene!");

                if (objs != null && objs.Length > 0)
                    instance = objs[0];

                if (instance == null)
                {
                    GameObject go = new GameObject($"[Singleton]{typeof(T).Name}");
                    instance = go.AddComponent<T>();
                }

                return instance;
            }           
        }
    }

    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        exitApplication = true;
        instance = null;
    }
}
