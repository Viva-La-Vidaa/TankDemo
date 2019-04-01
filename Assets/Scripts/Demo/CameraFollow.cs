using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;//相机跟随的目标

    public float smooth = 1.5f;
    private Vector3 relCameraPos;
    private Quaternion rotation;
    private Vector3 offset = new Vector3(-36, 48,-20);//视角调好的偏移量

    // Start is called before the first frame update
    void Start()
    {
        //默认Tank1为当前客户端玩家
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = player.position + offset;//初始化相机位置
        rotation = transform.rotation;
        relCameraPos = transform.position - player.position;
    }

    private void FixedUpdate()
    {
        if (player == null) return;
        Vector3 newPos =player.position+ relCameraPos;
        transform.position = Vector3.Lerp(transform.position,newPos,smooth*Time.deltaTime);
        transform.rotation = rotation;
    }
}
