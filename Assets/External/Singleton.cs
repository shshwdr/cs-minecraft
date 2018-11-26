using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T THE_INSTANCE;
    public static T Instance
    {
        get
        {
            if (!THE_INSTANCE)
            {
                THE_INSTANCE = (T)FindObjectOfType(typeof(T));
                if (FindObjectsOfType(typeof(T)).Length > 1)
                {
                    //CSUtil.ERROR("singleton there should never be more than 1 singleton" + typeof(T));
                }
                else if (!THE_INSTANCE)
                {
                    GameObject go = new GameObject();
                    THE_INSTANCE = go.AddComponent<T>();
                    //DontDestroyOnLoad(go);
                    //CSUtil.LOG("singleton " + typeof(T) + " create");
                }
                else
                {

                    //CSUtil.LOG("singleton " + typeof(T) + " has been created");
                }
            }
            return THE_INSTANCE;
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
