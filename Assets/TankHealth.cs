using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankHealth : MonoBehaviour
{
    public int Hp = 100;//坦克默认的血量
    public GameObject TankExplosion;
    public AudioClip TankExplosionAudio;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void TakeDamage() 
    {
        if (Hp <= 0) return;//如果血量已小于0,直接结束
        Hp -= Random.Range(10, 20);//如果血量大于0,血量减少,伤害在10-20之间
        if(Hp <= 0)//收到伤害之后 血量为0 控制死亡效果
        {
            AudioSource.PlayClipAtPoint(TankExplosionAudio, transform.position);
            GameObject.Instantiate(TankExplosion, transform.position + Vector3.up, transform.rotation);//实例化tankExplosion
            GameObject.Destroy(this.gameObject);
        }

        }
    }
