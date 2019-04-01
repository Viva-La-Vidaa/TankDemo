using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading; 
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

            for (long i=0; i<num; i++){
                long id = config_value.ids[i];
                if(id == net_game.net.GetPlayerId()){
                    SpawnPlayer(id, config_value.values[i].x, config_value.values[i].y);
                }else {
                    SpawnEnemy(id, config_value.values[i].x, config_value.values[i].y);
                    Enemy.SetID(id);
                    Debug.Log("生成敌人"+id);
                }
                config.Add(id);
            }
            Debug.Log("游戏初始化完成");

            Thread thread = new Thread(new ThreadStart(net_game.net.SetMoveValueByNet));//接受移动参数数据
            thread.Start();

            b = true;
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnEnemy(1, -15, 20);//这里假设服务端发送过来的是2号
        }
    }

    /// <summary>
    /// tank生成
    /// </summary>
    /// <param name="TankNo">坦克的编号</param>
    void SpawnPlayer(long TankNo, float x, float y)//生成玩家
    {
        GameObject go = Instantiate(playerPrefab) as GameObject;
        go.transform.tag = "Player";
        go.transform.name = "Tank" + TankNo;      
        Vector3 value = new Vector3(x,0,y);
        go.transform.position = value;
    }

    void SpawnEnemy(long TankNo, float x, float y)//生成敌人
    {
        GameObject go = Instantiate(enemyPrefab) as GameObject;
        go.transform.tag = "Enemy";
        go.transform.name = "Tank" + TankNo;
        Vector3 value = new Vector3(x,0,y);
        go.transform.position = value;
    }



}
