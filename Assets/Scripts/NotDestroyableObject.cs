using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotDestroyableObject : MonoBehaviour
{
    private static NotDestroyableObject instance = null;
    public static NotDestroyableObject Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
