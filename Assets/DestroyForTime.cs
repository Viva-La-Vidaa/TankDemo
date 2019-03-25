using UnityEngine;
using System.Collections;

public class DestroyForTime : MonoBehaviour
{
    public float LifetTime;//生存时间

    // Use this for initialization
    void Start()
    {
        Destroy(this.gameObject, LifetTime);//生存时间到了就销毁
    }

    // Update is called once per frame
    void Update()
    {

    }
}
