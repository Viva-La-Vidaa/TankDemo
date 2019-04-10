using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XXX1 : MonoBehaviour
{
    static public int x1 = 0;

    void FixedUpdate()
    {
        Debug.LogWarning("【2】: " + XXX.x +" "+ x1);
        x1++;
    }
}
