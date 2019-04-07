using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour{
    //坦克移动控制
    public float Speed = 5;
    public float AngulaSpeed = 15;
    private Rigidbody body;
    public Transform gameManagerTransform;

    public AudioClip idleAudio;
    public AudioClip drivingAudio;
    private AudioSource audioSource;//坦克移动声音

    public int Hp = 100;//坦克默认的血量
    public GameObject tankExplosion;
    public AudioClip tankExplosionAudio;

    public GameObject shellPrefab;
    public KeyCode fireKey = KeyCode.Space;//空格键开火
    private Transform firePosition;
    public float shellSpeed = 15;
    public AudioClip shotClip;
    public AudioSource audioSourceFire;

    private float _X;
    private float _Y;


    void Start()
    {
        body = this.GetComponent<Rigidbody>();
        audioSource = this.GetComponent<AudioSource>();
        InitOperation();
    }

    //初始化操作
    void InitOperation()
    {
        //炮弹起始位置
        firePosition = transform.Find ("FirePosition");
        //初始坦克位置
        System.Random random = new System.Random();
    }

    // Update is called once per frame
    void Update()
    {
        /* 
        if (Input.GetKeyDown(fireKey))
        {
            //AudioSource.PlayClipAtPoint(shotClip, transform.position, 1.0f);//开火的声音
            audioSourceFire.clip = shotClip;
            if (audioSource.isPlaying == false)
                audioSource.Play();
            GameObject go = GameObject.Instantiate(shellPrefab, firePosition.position, firePosition.rotation) as GameObject;
            //设置炮弹的父物体
            go.transform.parent = gameManagerTransform;
            go.GetComponent<Rigidbody>().velocity = go.transform.forward * shellSpeed;//炮弹速度
        }
        */
    }

    void FixedUpdate()
    {
        GameObject player = this.gameObject;
        if(player == null){
            Debug.LogError("this.GameObject错误");
            return ;
        }
        long _EnemyID = config.Get_id_by_Tag(player.name);

        //统一位置
        //GameObject player = GameObject.Find("Tank"+_EnemyID);
        float l = Vector3.Distance(player.transform.position,config.xyz_Get_by_id(_EnemyID).xz);//距离差值
        if(l > 3){
            player.transform.SetPositionAndRotation(config.xyz_Get_by_id(_EnemyID).xz, config.xyz_Get_by_id(_EnemyID).y);
            Debug.LogWarning("同步位置");
        }

        //移动
        _X = config.Get_xy_by_id(_EnemyID).x;
        _Y = config.Get_xy_by_id(_EnemyID).y;         
        body.angularVelocity = transform.up * _X * AngulaSpeed;
        body.velocity = transform.forward *_Y * Speed;

        if (Mathf.Abs(_X) > 0.1 || Mathf.Abs(_Y) > 0.1)  //坦克行走时播放的声音
        {
            audioSource.clip = drivingAudio;
            if (audioSource.isPlaying == false)
                audioSource.Play();
        }
        else //坦克停止时播放的声音                                                  
        {
            audioSource.clip = idleAudio;
            if (audioSource.isPlaying == false)
                audioSource.Play();
        }      
    }

    //Tank伤害计算
    void TakeDamage()
    {
        /* 
        //如果血量已小于0,直接结束
        if (Hp <= 0)
            return;
        //如果血量大于0,血量减少,伤害在10-20之间
        Hp -= Random.Range(10, 20);
        //收到伤害之后 血量为0 控制死亡效果
        if (Hp <= 0)
        {
            AudioSource.PlayClipAtPoint(tankExplosionAudio, transform.position);
            GameObject.Instantiate(tankExplosion, transform.position + Vector3.up, transform.rotation);//实例化tankExplosion
            GameObject.Destroy(this.gameObject);
        }
        */
    }
}
