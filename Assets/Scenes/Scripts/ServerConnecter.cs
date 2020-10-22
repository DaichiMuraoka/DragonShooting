using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ServerConnecter : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject popupBackPanel;
    //マスターサーバーへ接続
    public void ConnectServer(string playerName)
    {
        //プレイヤー名設定
        PhotonNetwork.LocalPlayer.NickName = playerName;
        //サーバー接続
        PhotonNetwork.ConnectUsingSettings();
    }
    //マスターサーバー接続コールバック
    public override void OnConnectedToMaster()
    {
        PopupMessage("マスターサーバーに接続しました");
        //ロビーに入室する
        PhotonNetwork.JoinLobby();
    }
    //サーバーから切断
    public void DisconnectServer()
    {
        PhotonNetwork.Disconnect();

    }
    //ルーム作成
    public void CreateRoom(string roomName, string message)
    {
        PopupMessage("ルームを作成します");
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable()
        {
            {"DisplayName", roomName}, {"Message", message}
        };
        string[] crpfl = new[]
        {
            "DisplayName",
            "Message"
        };
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 2, CustomRoomProperties = hashtable, CustomRoomPropertiesForLobby = crpfl };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }
    //ルームの作成成功コールバック
    public override void OnCreatedRoom()
    {
        PopupMessage("ルーム作成に成功しました");
    }
    //ルーム作成失敗コールバック
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        PopupMessage($"ルーム作成に失敗しました: {message}");
    }
    //ルームに参加する
    public void JoinRoom(string roomName)
    {
        PopupMessage("ルームに参加しています。");
        PhotonNetwork.JoinRoom(roomName);
    }
    //ポップアップメッセージ表示
    public void PopupMessage(string message)
    {
        if (!popupBackPanel.activeSelf)
        {
            popupBackPanel.SetActive(true);
        }
        Debug.Log(message);
        popupBackPanel.transform.Find("PopupText").GetComponent<Text>().text = message;
        StartCoroutine(DeletePopup());
    }
    //ポップアップメッセージ非表示
    IEnumerator DeletePopup()
    {
        yield return new WaitForSeconds(2);
        if (popupBackPanel.activeSelf)
        {
            popupBackPanel.SetActive(false);
        }
    }
    //他プレイヤー参加時コールバック
    public override void OnPlayerEnteredRoom(Player player)
    {
        Debug.Log(player.NickName + "が参加しました");
    }
    //他プレイヤー退出時コールバック
    public override void OnPlayerLeftRoom(Player player)
    {
        Debug.Log(player.NickName + "が退出しました");
    }
}
