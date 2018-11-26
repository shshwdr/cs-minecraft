using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSUtil : MonoBehaviour {

    public static void LOG(object message)
    {
        Debug.Log("(color switch)" + message.ToString());
    }

    public static void ERROR(object message)
    {
        Debug.LogError("(color switch)" + message.ToString());
    }

}
