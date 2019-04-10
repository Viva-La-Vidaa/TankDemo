using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    static public int o = 0;
    static public int F = 0;
    static public int my= 0;
    private float oldq = 0;
    private float oldv = 0;
    void FixedUpdate()
    {
        if(net_game.net.GetGameing()){
            GameObject player = GameObject.Find(net_game.net.GetPlayerId().ToString());
            float q = Input.GetAxisRaw("Horizontal");//获取纵轴轴向
            float v = Input.GetAxisRaw("Vertical");//获取纵轴轴向

            if(q != oldq || v != oldv){
                if(Input.GetAxisRaw("Horizontal") == q && Input.GetAxisRaw("Vertical") == v){
                        Debug.LogWarning("服务器在第 " + my + " 帧状态改变" );//当前是第几帧
                        Debug.LogWarning("服务器经上次状态维持了几帧：" + ( my - F ) );//经过几帧
                        net_game.Move(q, v, ( my - F ));
                        o = F;
                        F = my;//客户端在帧转态改变
                        oldq = q;
                        oldv = v;
                }
            }
            my++;//My表示当前的帧数，从零开始
        }
    }
}

