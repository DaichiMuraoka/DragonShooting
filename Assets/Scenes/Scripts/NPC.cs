using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NPC : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private BulletController bullet = null;
    [SerializeField]
    private Item item = null;
    [SerializeField]
    private Item bonusItem = null;
    private bool fire = true;
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
    public void StopFire()
    {
        fire = false;
    }
    IEnumerator Update1()
    {
        int count = 0;
        while (fire)
        {
            photonView.RPC(nameof(NPCFire), RpcTarget.All, count);
            count++;
            yield return new WaitForSeconds(2.0f);
        }
    }
    [PunRPC]
    private void NPCFire(int count, PhotonMessageInfo info)
    {
        for (int rad = 0 + count; rad < 360 + (count * 12); rad += 12)
        {
            int timestamp = info.SentServerTimestamp;               //射出時間の取得
            float rdm = Random.Range(0f, 1.0f);
            if (rdm <= 0.05f)
            {
                Item i = Instantiate(bonusItem);
                i.Init(transform.position, rad, timestamp);
            }
            else if(rdm <= 0.25)
            {
                Item i = Instantiate(item);
                i.Init(transform.position, rad, timestamp);
            }
            else
            {
                Debug.Log("Npc Fire");
                BulletController ability = Instantiate(bullet);
                ability.Init(transform.position, rad, timestamp);  //弾のセッティング
            }
        }
    }
}
