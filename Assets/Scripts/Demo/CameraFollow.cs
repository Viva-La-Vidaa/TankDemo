using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;//相机跟随的目标

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        //默认Tank1为当前客户端玩家
        player = GameObject.Find("Tank1").transform;
        offset = transform.position - player.position / 2;
    }

    private void LateUpdate()
    {
        if (player == null) return;
        transform.position = player.position / 2 + offset;
    }
}
