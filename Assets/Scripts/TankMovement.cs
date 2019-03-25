using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public float Speed = 5;
    public float AngulaSpeed = 15;
    public float PlayerNum = 1;          //玩家编号,用于区分不同的控制
    private Rigidbody body;

    public AudioClip IdleAudio;
    public AudioClip DrivingAudio;
    private AudioSource Audio;

    void Start()
    {
        body = this.GetComponent<Rigidbody>();
        Audio = this.GetComponent<AudioSource>();
    }
   
    void FixedUpdate()
    {
        //旋转
        float x = Input.GetAxis("HorizontalPlayer"+ PlayerNum);//获取横轴轴向
        body.angularVelocity = transform.up * x* AngulaSpeed;

        //前进后退
        float y = Input.GetAxis("VerticalPlayer"+ PlayerNum);//获取纵轴轴向
        body.velocity = transform.forward *y * Speed;
        if (Mathf.Abs(y) > 0.1 || Mathf.Abs(x) > 0.1)  //坦克行走时播放的声音
        {
            Audio.clip = DrivingAudio;
            if (Audio.isPlaying == false)
                Audio.Play();
        }
        else                                                   //坦克停止时播放的声音
        {
            Audio.clip = IdleAudio;
            if (Audio.isPlaying == false)
                Audio.Play();
        }
    }
}
