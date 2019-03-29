using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    int i = 0;
    bool b = false;
    // Update is called once per frame
    void Update()
    {
        if(!net_game.net.GetGameing()){
            Debug.Log("等待游戏开始" + i++);//替换为等待场景
        }else if(!b){
            Debug.Log("游戏开始");
            net_game.net.GameMsgInit();//初始化场景
            CONFIG.Config_Value config_value = net_game.net.GetGameinit();
            long num = config_value.num;
            for(long i=0; i<num; i++){
                SpawnEnemy(config_value.ids[i]);
                Debug.Log("生成坦克"+config_value.ids[i]);
            }
            Debug.Log("游戏初始化完成");
            b = true;
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnPlayer(1);//这里假设服务端发送过来的是2号
        }
    }

    /// <summary>
    /// tank生成
    /// </summary>
    /// <param name="TankNo">坦克的编号</param>
    void SpawnPlayer(long TankNo)//生成玩家
    {
        GameObject go = Instantiate(playerPrefab) as GameObject;
        go.transform.name = "Tank" + TankNo;
    }

    void SpawnEnemy(long TankNo)//生成敌人
    {
        GameObject go = Instantiate(enemyPrefab) as GameObject;
        go.transform.name = "Tank" + TankNo;

        //区分本地玩家与其它客户端玩家的特效处理
    }



}
