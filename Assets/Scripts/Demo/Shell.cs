﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public GameObject shellExplosionPrefab;//实例化爆炸特效
    public AudioClip shellExplosionAudio;
    private Transform gameManagerTransform; //游戏管理器

    private void Start()
    {
        gameManagerTransform = GameObject.Find("GameManager").transform;
    }

    public void OnTriggerEnter(Collider collider)//触发检测
    {
        AudioSource.PlayClipAtPoint(shellExplosionAudio, transform.position);
        GameObject go = GameObject.Instantiate(shellExplosionPrefab, transform.position, transform.rotation);//先实例化特效
        go.transform.parent = gameManagerTransform;
        GameObject.Destroy(this.gameObject);//再自身销毁
        if (collider.tag == "Enemy")//如果炮弹碰撞到坦克,则对坦克造成伤害
        {
            collider.SendMessage("TakeDamage");
            //显示击中Tanke的UI
            Tank enemyTank = collider.gameObject.GetComponent<Tank>();
            if (enemyTank.hpValue <= 0)
            {
                Tank playerTank = GameObject.FindWithTag("Player").GetComponent<Tank>();
                playerTank.expValue += 50;
                //更新经验值
                UIManager.Instance.ShowPlayerUI(playerTank.userName, playerTank.levelValue, playerTank.hpValue, playerTank.expValue);
            }
            else
            {
                UIManager.Instance.ShowEnemyUI(enemyTank.userName, enemyTank.levelValue, enemyTank.hpValue, enemyTank.expValue);
            }
        }
        else
        {
            UIManager.Instance.HideEnemyUI();
        }
    }

}
