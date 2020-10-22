using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject hiteffect;
    [SerializeField]
    private int power = 10;     //弾の威力
    [SerializeField]
    private float speed = 9f;   //弾のスピード
    [SerializeField]
    private float coolTime = 1.0f;    //射出クールタイム
    [SerializeField]
    private int bulletID;   //ID
    private Vector3 velocity;
    public void Init(Vector3 origin, float angle, int timestamp)
    {
        transform.position = origin;
        velocity = speed * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        OnUpdate(origin, timestamp);
    }

    private void Update()
    {
        var dv = velocity * Time.deltaTime;
        transform.Translate(dv.x, dv.y, 0f);
    }

    private void OnUpdate(Vector3 origin, int timestamp)
    {
        float elapsedTime = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);
        transform.position = origin + velocity * elapsedTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if((this.tag == "PlayerBullet" && collision.gameObject.tag == "Enemy")||(this.tag == "EnemyBullet" && collision.gameObject.tag == "Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().OnDamage(power);
            Instantiate(hiteffect, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
    public float GetCoolTime()
    {
        return coolTime;
    }
    public float GetSpeed()
    {
        return speed;
    }
    public int GetPower()
    {
        return power;
    }

    public int GetID()
    {
        return bulletID;
    }
    public void SetSEVolume(float volume)
    {
        GetComponent<AudioSource>().volume = volume;
        hiteffect.GetComponent<AudioSource>().volume = volume;
    }
}
