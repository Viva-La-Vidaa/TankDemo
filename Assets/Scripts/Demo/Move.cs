using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private float oldx = 0;
    private float oldy = 0;
    void FixedUpdate()
    {
        if(net_game.net.GetRooming()){
            GameObject player = GameObject.Find(net_game.net.GetPlayerId().ToString());
            float x = Input.GetAxisRaw("Horizontal");//获取纵轴轴向
            float y = Input.GetAxisRaw("Vertical");//获取纵轴轴向

            if(x != oldx || y != oldy){
                if(Input.GetAxisRaw("Horizontal") == x && Input.GetAxisRaw("Vertical") == y){
                        net_game.Move(x, y);
                        oldx = x;
                        oldy = y;
                }
            }
        }
    }
}

