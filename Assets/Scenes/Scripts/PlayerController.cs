
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private int hp = 100;   //hp
    [SerializeField]
    private float speed = 10.0f;    //移動スピード
    private Rigidbody2D rb;     //Rigidbody2D
    //弾1
    [SerializeField]
    private BulletController bullet1Prefab = default;    //プレファブ
    private float bullet1CoolTime = 1.0f;    //射出クールタイム
    private bool canFireBullet1 = false;     //射出クールタイムかどうか
    private float bullet1Charge = 0f;        //射出クールタイム計測用
    //弾2
    [SerializeField]
    private BulletController bullet2Prefab = default;    //プレファブ
    private float bullet2CoolTime = 1.0f;    //射出クールタイム
    private bool canFireBullet2 = false;     //射出クールタイムかどうか
    private float bullet2Charge = 0f;        //射出クールタイム計測用
    //非装備弾
    [SerializeField]
    private List<BulletController> otherBulletPrefabs;
    //向き
    private Vector3 stscale;    //開始時点の向き(右向き)
    private Vector3 scale;      //現在の向き
    [SerializeField]
    private GameObject damageTextPrefab;    //ダメージテキストプレファブ
    [SerializeField]
    private Vector3 damageTextOffset = new Vector3(0f, 0.8f, 0f);   //ダメージテキストの表示位置
    [SerializeField]
    private GameObject deathEffect; //敗北エフェクト
    private bool wait = true;       //待機フラグ
    private Slider hpSlider;        //HPスライダー（自キャラ用）
    private BattleManager battleManager = null;

    //画面端の座標
    private Vector2 screenLeftBottom = new Vector2(0, 0);
    private Vector2 screenRightTop = new Vector2(0, 0);
    private void Start()
    {
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        rb = GetComponent<Rigidbody2D>();
        //敵キャラのタグをEnemyに変更
        if (!photonView.IsMine)
        {
            tag = "Enemy";
        }
        //弾のクールタイム設定
        bullet1CoolTime = bullet1Prefab.GetCoolTime();
        bullet2CoolTime = bullet2Prefab.GetCoolTime();
        //画面端座標取得
        screenLeftBottom = Camera.main.ScreenToWorldPoint(Vector2.zero);
        screenRightTop = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }
    void Update()
    {
        //自キャラ以外は動かせない
        if (!photonView.IsMine)
        {
            return;
        }
        //待機中は動かせない
        if (wait)
        {
            return;
        }
        //射出クールタイム更新
        bullet1Charge = Math.Min(bullet1Charge + Time.deltaTime, bullet1CoolTime);
        bullet2Charge = Math.Min(bullet2Charge + Time.deltaTime, bullet2CoolTime);
        if (bullet1Charge == bullet1CoolTime && !canFireBullet1)
        {
            canFireBullet1 = true;
            Debug.Log("can fire bullet1!");
        }
        if (bullet2Charge == bullet2CoolTime && !canFireBullet2)
        {
            canFireBullet2 = true;
            Debug.Log("can fire bullet2!");
        }
        //方向キー入力検知
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector2 dir = new Vector2(h, v).normalized;
        Vector2 dv = speed * Time.deltaTime * dir;
        //画面端は動かない
        Vector2 nextPosition = new Vector2(transform.position.x + dv.x, transform.position.y + dv.y);
        if(nextPosition.x > screenLeftBottom.x && nextPosition.x < screenRightTop.x
            && nextPosition.y < screenRightTop.y && nextPosition.y > screenLeftBottom.y)
        {
            //移動
            transform.Translate(dv);
        }
        //クリック時
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Vector2 dp = GetFireAngle();
            //向きを更新
            scale = GetComponent<Transform>().localScale;
            if ((dp.x < 0 && scale == stscale) || (dp.x > 0 && scale != stscale))
            {
                Debug.Log(scale);
                Debug.Log(stscale);
                SwichPlayersDirection();
            }
            if (Input.GetMouseButtonDown(0) && canFireBullet1)
            {
                //弾1発射処理
                //射出クールタイムリセット
                bullet1Charge = 0f;
                canFireBullet1 = false;
                //弾1発射
                float angle = Mathf.Atan2(dp.y, dp.x);
                photonView.RPC(nameof(FireProjectile), RpcTarget.All, angle, bullet1Prefab.GetID());
            }
            else if (Input.GetMouseButtonDown(1) && canFireBullet2)
            {
                //弾2発射処理
                //射出クールタイムリセット
                bullet2Charge = 0f;
                canFireBullet2 = false;
                //弾2発射
                float angle = Mathf.Atan2(dp.y, dp.x);
                photonView.RPC(nameof(FireProjectile), RpcTarget.All, angle, bullet2Prefab.GetID());
            }
        }
    }
    //マウスの方向取得
    public Vector2 GetFireAngle()
    {
        //射出角度の計算
        Vector2 playerWorldPosition = transform.position;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dp = mouseWorldPosition - playerWorldPosition;
        return dp;
    }
    //向きの反転
    public void SwichPlayersDirection()
    {
        GetComponent<Transform>().localScale = new Vector2(-scale.x, scale.y);
        photonView.RPC(nameof(SwitchNameIcon), RpcTarget.All);
    }
    //名前アイコンの向き変更
    [PunRPC]
    public void SwitchNameIcon()
    {
        Vector3 nameTransform = transform.Find("Icon").transform.Find("PlayerNameText").GetComponent<Transform>().localScale;
        transform.Find("Icon").transform.Find("PlayerNameText").GetComponent<Transform>().localScale = new Vector3(-nameTransform.x, nameTransform.y, nameTransform.z);
    }
    //初期向きの設定
    public void SetStartScale()
    {
        stscale = GetComponent<Transform>().localScale;
        scale = GetComponent<Transform>().localScale;
    }
    //射出
    [PunRPC]
    private void FireProjectile(float angle, int bulletID, PhotonMessageInfo info)
    {
        //弾の生成
        BulletController projectile = battleManager.GetAbility(bulletID);
        if(projectile == null)
        {
            Debug.Log("Error");
            return;
        }
        BulletController ability = Instantiate(projectile);
        Debug.Log("Fire!");
        //弾のタグ設定
        if(tag == "Player")
        {
            ability.tag = "PlayerBullet";
        }
        else
        {
            ability.tag = "EnemyBullet";
        }
        int timestamp = info.SentServerTimestamp;               //射出時間の取得
        ability.Init(transform.position, angle, timestamp);  //弾のセッティング
    }
    //オブジェクト状態変化コールバック
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        PhotonNetwork.SendRate = 30;            //1秒間にメッセージ送信を行う回数
        PhotonNetwork.SerializationRate = 30;   //1秒間にオブジェクト同期を行う回数
        if (stream.IsWriting)
        {
            //自分の変化を送信
            stream.SendNext(transform.position);
            stream.SendNext(transform.localScale);
        }
        else
        {
            //相手の変化を受信&更新
            transform.position = (Vector3)stream.ReceiveNext();
            transform.localScale = (Vector3)stream.ReceiveNext();
        }
    }
    //被弾処理
    public void OnDamage(int damage)
    {
        hp = Math.Max(hp - damage, 0);  //hp減少
        photonView.RPC(nameof(InstantiateDamageText), RpcTarget.All, damage);   //ダメージテキストの生成
        //HPスライダーの更新
        if (photonView.IsMine)
        {
            hpSlider.value = hp;
        }
        //hp0なら敗北処理
        if(hp == 0)
        {
            Death();
        }
    }
    //ダメージテキスト生成処理
    [PunRPC]
    private void InstantiateDamageText(int damage)
    {
        Instantiate(damageTextPrefab, transform.position + damageTextOffset, Quaternion.identity).GetComponent<TextMesh>().text = damage.ToString();
    }
    //敗北処理
    [PunRPC]
    private void Death()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);     //敗北エフェクト
        BattleManager bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        if (photonView.IsMine)
        {
            bm.GameOverProcess(false);
        }
        else
        {
            bm.GameOverProcess(true);
        }
        Destroy(gameObject);   //キャラを削除
    }
    //waitの変更
    public void SwitchWait(bool _wait)
    {
        wait = _wait;
    }
    //hpスライダー
    public void SetHPSlider(Slider slider)
    {
        hpSlider = slider;
        hpSlider.maxValue = hp;
        hpSlider.value = hpSlider.maxValue;
    }
    public BulletController GetAbility(int number)
    {
        if(number == 1)
        {
            return bullet1Prefab;
        }
        else if(number == 2)
        {
            return bullet2Prefab;
        }
        else
        {
            return null;
        }
    }
    public void SetAbility(BulletController ability, int number)
    {
        if(number == 1)
        {
            bullet1Prefab = ability;
            return;
        }
        if(number == 2)
        {
            bullet2Prefab = ability;
            return;
        }
        if(number == 3)
        {
            otherBulletPrefabs.Add(ability);
        }
    }
    public List<BulletController> GetOtherAbilities()
    {
        return otherBulletPrefabs;
    }
    public void ResetAbility()
    {
        bullet1Prefab = null;
        bullet2Prefab = null;
        otherBulletPrefabs.Clear();
    }
    public void RemoveAbility(string abilityName)
    {
        if(otherBulletPrefabs.Count > 0)
        {
            foreach (BulletController ability in otherBulletPrefabs)
            {
                if(ability.name == abilityName)
                {
                    otherBulletPrefabs.Remove(ability);
                    break;
                }
            }
        }
    }
    public string GetStringData()
    {
        string data = name;
        if(bullet1Prefab != null)
        {
            data += ":";
            data += bullet1Prefab.name;
        }
        if(bullet2Prefab != null)
        {
            data += ":";
            data += bullet2Prefab.name;
        }
        if(otherBulletPrefabs.Count > 0)
        {
            foreach(BulletController ability in otherBulletPrefabs)
            {
                data += ":";
                data += ability.name;
            }
        }
        return data;
    }
    public void SetSEVolume(float volume)
    {
        bullet1Prefab.SetSEVolume(volume);
    }
    public void SetPlayerName(string name)
    {
        photonView.RPC(nameof(SetPlayerNameRPC), RpcTarget.All, name);
    }

    [PunRPC]
    public void SetPlayerNameRPC(string name)
    {
        transform.Find("Icon").transform.Find("PlayerNameText").GetComponent<TextMesh>().text = name;
    }
}
