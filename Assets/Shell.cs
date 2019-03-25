using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public GameObject ShellExplosionPrefab;//实例化爆炸特效
    public AudioClip ShellExplosionAudio;

    public void OnTriggerEnter(Collider collider)//触发检测
    {
        AudioSource.PlayClipAtPoint(ShellExplosionAudio, transform.position);
        GameObject.Instantiate(ShellExplosionPrefab, transform.position, transform.rotation);//先实例化特效
        GameObject.Destroy(this.gameObject);//再自身销毁

        if (collider.tag == "Tank")//如果炮弹碰撞到坦克,则对坦克造成伤害
        {
            collider.SendMessage("TakeDamage");
        }
    }

}
