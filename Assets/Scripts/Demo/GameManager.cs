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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnEnemy(2);//这里假设服务端发送过来的是2号
        }
    }

    /// <summary>
    /// tank生成
    /// </summary>
    /// <param name="TankNo">坦克的编号</param>
    void SpawnPlayer(int TankNo)//生成玩家
    {
        GameObject go = Instantiate(playerPrefab) as GameObject;
        go.transform.name = "Tank" + TankNo;
    }

    void SpawnEnemy(int TankNo)//生成敌人
    {
        GameObject go = Instantiate(enemyPrefab) as GameObject;
        go.transform.name = "Tank" + TankNo;

        //区分本地玩家与其它客户端玩家的特效处理
    }



}
