using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAttack : MonoBehaviour
{
    public GameObject ShellPrefab;
    public KeyCode FireKey = KeyCode.Space;//空格键开火
    private Transform FirePosition;
    public float ShellSpeed = 15;
    public AudioClip ShotAudio;

    void Start()
    {
        FirePosition = transform.Find("FirePosition");//炮弹起始位置
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(FireKey))
        {
            AudioSource.PlayClipAtPoint(ShotAudio, transform.position);//开火的声音
            GameObject go = GameObject.Instantiate(ShellPrefab, FirePosition.position, FirePosition.rotation) as GameObject;
            go.GetComponent<Rigidbody>().velocity = go.transform.forward * ShellSpeed;//炮弹速度
        }
    }
}
