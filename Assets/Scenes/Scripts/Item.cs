using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private int score = 10;
    [SerializeField]
    private int id = 0;
    [SerializeField]
    private float speed = 9f;   //弾のスピード

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
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            PlayerController p = collision.gameObject.GetComponent<PlayerController>();
            p.AddScore(score);
            Destroy(this.gameObject);
        }
    }
    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
