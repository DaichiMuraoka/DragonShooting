using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NPC : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private BulletController bullet = null;
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
    public void SetUp()
    {
        if(bullet == null)
        {
            return;
        }
        StartCoroutine(Update1());
    }
    IEnumerator Update1()
    {
        int count = 0;
        while (true)
        {
            for(int rad = 0; rad < 360; rad += 6)
            {
                Debug.Log("Npc Fire");
                photonView.RPC(nameof(NPCFire), RpcTarget.All, (float)rad);
            }
            yield return new WaitForSeconds(3.0f);
            count++;
        }
    }
    [PunRPC]
    private void NPCFire(float angle, PhotonMessageInfo info)
    {
        BulletController ability = Instantiate(bullet);
        int timestamp = info.SentServerTimestamp;               //射出時間の取得
        ability.Init(transform.position, angle, timestamp);  //弾のセッティング
    }
}
