using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Tank : MonoBehaviour
{
    public int expValue = 0; //游戏中的经验值
    public int levelValue = 1; //默认一级
    public string nickName;//游戏呢称

    //坦克移动控制
    public float speed = 5;
    public float angulaSpeed = 15;
    public float playerNum = 1;    //玩家编号,用于区分不同的控制
    private Rigidbody body;
    public Transform gameManagerTransform;

    public AudioClip idleAudio;
    public AudioClip drivingAudio;
    private AudioSource audioSource;//坦克移动声音

    public int Hp = 100;//坦克默认的血量
    public GameObject tankExplosion;
    public AudioClip tankExplosionAudio;
    public Slider hpSlider;//血条

    public GameObject shellPrefab;
    public KeyCode fireKey = KeyCode.Space;//空格键开火
    private Transform firePosition;
    public float shellSpeed = 15;
    public AudioClip shotClip;
    public AudioSource audioSourceFire;

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
        firePosition = transform.Find("FirePosition");
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    void FixedUpdate()
    {
        if(net_game.net.GetRooming()){
            GameObject player = GameObject.Find(net_game.net.GetPlayerId().ToString());
            //旋转
            float x = Input.GetAxis("HorizontalPlayer1");//获取横轴轴向
            if(x  != 0 && (x < 1 && x > -1)){
                net_game.Move_X(x, player.transform.position, player.transform.rotation);
            }else if(x == 0 && net_game.net.Get_moveing_x()){
                net_game.Move_X(x, player.transform.position, player.transform.rotation);
            }else if((x == 1 && config.Get_xy_by_id(net_game.net.GetPlayerId()).x != 1 ) ||  (x == -1 && config.Get_xy_by_id(net_game.net.GetPlayerId()).x != -1)){
                net_game.Move_X(x, player.transform.position, player.transform.rotation);
            }
            body.angularVelocity = transform.up * config.Get_xy_by_id(net_game.net.GetPlayerId()).x * angulaSpeed;

            //前进后退
            float y = Input.GetAxis("VerticalPlayer1");//获取纵轴轴向
            if( y != 0){
                net_game.Move_Y(y, player.transform.position, player.transform.rotation);
            }else if(y == 0 && net_game.net.Get_moveing_y()){
                net_game.Move_Y(y, player.transform.position, player.transform.rotation);
            }
            body.velocity = transform.forward *config.Get_xy_by_id(net_game.net.GetPlayerId()).y * speed;

            if (Mathf.Abs(y) > 0.1 || Mathf.Abs(x) > 0.1)  //坦克行走时播放的声音
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
    }

    //Tank伤害计算
    void TakeDamage()
    {
        //如果血量已小于0,直接结束
        if (Hp <= 0)
            return;
        //如果血量大于0,血量减少,伤害在10-20之间
        Hp -= Random.Range(10, 20);
        hpSlider.value = Hp / 100;
        //收到伤害之后 血量为0 控制死亡效果
        if (Hp <= 0)
        {
            AudioSource.PlayClipAtPoint(tankExplosionAudio, transform.position);
            GameObject.Instantiate(tankExplosion, transform.position + Vector3.up, transform.rotation);//实例化tankExplosion
            GameObject.Destroy(this.gameObject);
        }
    }
}
