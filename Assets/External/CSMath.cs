using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSMath : MonoBehaviour {

	public static bool Vector3Equal(Vector3 v1,Vector3 v2, float eps = 0.2f)
    {
        float dis = Vector3.Distance(v1, v2);
        if (dis <= eps)
        {
            return true;
        }
        return false;
    }

    public static Vector3 Vector3Inf = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
}
