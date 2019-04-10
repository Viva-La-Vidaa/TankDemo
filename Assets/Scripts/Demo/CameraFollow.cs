using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;//相机跟随的目标

    public float smooth = 1.5f;
    private Vector3 relCameraPos;
    private Quaternion rotation;
    private Vector3 offset = new Vector3(-36, 48,-20);//视角调好的偏移量
    private bool flag = true;

    // Start is called before the first frame update
    void Start()
    {
        rotation = transform.rotation;
    }

    private void Update()
    {
        //只执行一次 之所以不放在start里面，是由于脚本执行的顺序问题
        if (player != null && flag)
        {
            transform.position = player.position + offset;//初始化相机位置
            relCameraPos = transform.position - player.position;
            flag = false;
        }
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        Vector3 newPos =player.position+ relCameraPos;
        transform.position = Vector3.Lerp(transform.position,newPos,smooth*Time.deltaTime);
        transform.rotation = rotation;
    }
}
