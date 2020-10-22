using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Asteroids;

public class RoomButton : MonoBehaviourPunCallbacks
{
    private string roomName = "";
    public void SetRoomName(string newRoomName)
    {
        roomName = newRoomName;
    }
    public string GetRoomName()
    {
        return roomName;
    }
    public void OnClick()
    {
        GameObject serverConnecter = GameObject.Find("ServerConnecter");
        if(serverConnecter == null)
        {
            Debug.Log("LobbyManager is not find.");
            return;
        }
        serverConnecter.GetComponent<ServerConnecter>().JoinRoom(roomName);
    }
}
