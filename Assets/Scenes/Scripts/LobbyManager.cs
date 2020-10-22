using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject roomButtonContent;
    [SerializeField]
    private GameObject roomButtonPrefab;
    [SerializeField]
    private GameObject createRoomBackPanel;
    [SerializeField]
    private InputField roomNameInputField;
    [SerializeField]
    private InputField roomMessageInputField;
    [SerializeField]
    private ServerConnecter serverConnecter;
    [SerializeField]
    private Button customRoomButton;
    [SerializeField]
    private Button updateRoomButton;
    [SerializeField]
    private GameObject popupBackPanel;
    [SerializeField]
    private GameObject matchingBackPanel;
    private bool matching = false;
    private List<RoomInfo> currentRoomList;
    [SerializeField]
    private BGMManager bgmManager;
    [SerializeField]
    private SaveDataManager saveDataManager;
    [SerializeField]
    private Button MatchingCancelButton;
    [SerializeField]
    private AudioClip clickSound;
    private AudioSource audioSource;
    [SerializeField]
    private GameObject loadingPanel;
    [SerializeField]
    private Text winCountText;
    private void Start()
    {
        loadingPanel.SetActive(true);
        Screen.SetResolution(960, 540, false, 60);
        createRoomBackPanel.SetActive(false);
        popupBackPanel.SetActive(false);
        matchingBackPanel.SetActive(false);
        PlayerDataAsset playerDataAsset = saveDataManager.GetPlayerDataAsset();
        serverConnecter.ConnectServer(playerDataAsset.GetPlayersName());
        bgmManager.SetBGMVolume(playerDataAsset.GetBGMVolume());
        bgmManager.RandomPlay();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = playerDataAsset.GetSEVolume();
        audioSource.clip = clickSound;
        loadingPanel.SetActive(false);
        winCountText.text = playerDataAsset.GetWinCount().ToString();
    }
    //ルーム作成ボタン押下時
    public void PushCustomRoomButton()
    {
        createRoomBackPanel.SetActive(true);
        audioSource.Play();
    }
    //ルーム作成
    public void CreateRoom()
    {
        audioSource.Play();
        string roomDisplayName = roomNameInputField.text;
        if(roomDisplayName == "")
        {
            PopupMessage("そのルーム名は使用できません");
        }
        else
        {
            string roomMessage = roomMessageInputField.text;
            CreateRoomButton("local", roomDisplayName, roomMessage, "1", "2");
            serverConnecter.CreateRoom(roomDisplayName, roomMessage);
        }
        roomNameInputField.text = "";
        roomMessageInputField.text = "";
        createRoomBackPanel.SetActive(false);
    }
    //ルームリストが更新された時に呼ばれるコールバック
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        currentRoomList = roomList;
    }
    //ルームリスト表示更新
    public void RoomListUpdate()
    {
        DeleteAllRoomButton();
        if (currentRoomList == null)
        {
            return;
        }
        foreach (RoomInfo info in currentRoomList)
        {
            string roomName = info.Name;
            string roomDisplayName = (string)info.CustomProperties["DisplayName"];
            string roomMessage = (string)info.CustomProperties["Message"];
            string playerCount = info.PlayerCount.ToString();
            string maxPlayers = info.MaxPlayers.ToString();
            if (roomDisplayName != null)
            {
                Debug.Log(roomDisplayName + ":" + playerCount + "/" + maxPlayers);
                CreateRoomButton(roomName, roomDisplayName, roomMessage, playerCount, maxPlayers);
            }
        }
    }
    //ルームボタン作成
    public void CreateRoomButton(string roomName, string roomDisplayName, string roomMessage, string playerCount, string maxPlayers)
    {
        GameObject roomButton = Instantiate(roomButtonPrefab, roomButtonContent.transform);
        roomButton.transform.Find("RoomNameText").GetComponent<Text>().text = roomDisplayName;
        roomButton.transform.Find("RoomMessageText").GetComponent<Text>().text = roomMessage;
        roomButton.transform.Find("PlayerCountText").GetComponent<Text>().text = playerCount.ToString();
        roomButton.transform.Find("MaxPlayersText").GetComponent<Text>().text = maxPlayers.ToString();
        roomButton.GetComponent<RoomButton>().SetRoomName(roomName);
    }
    //ルームボタン削除
    public void DeleteAllRoomButton()
    {
        foreach(Transform roomButton in roomButtonContent.transform)
        {
            Destroy(roomButton.gameObject);
        }
    }
    //ルーム更新ボタン押下時
    public void OnUpdateRoomButton()
    {
        audioSource.Play();
        PopupMessage("ルームリストを更新します");
        UpdateRoom();
    }
    //ルーム更新
    public void UpdateRoom()
    {
        RoomListUpdate();
        updateRoomButton.interactable = false;
        StartCoroutine(ReactiveUpdateRoomButton());
    }
    //ルーム更新ボタンの再アクティブ
    IEnumerator ReactiveUpdateRoomButton()
    {
        yield return new WaitForSeconds(3);
        updateRoomButton.interactable = true;
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
    //ルーム参加コールバック
    public override void OnJoinedRoom()
    {
        Debug.Log("ルームに参加しました");
        MatchingCancelButton.interactable = true;
        matching = true;
    }
    //マッチング完了処理
    IEnumerator MatchingComplete()
    {
        string matchingText = "マッチング完了";
        Debug.Log(matchingText);
        matchingBackPanel.transform.Find("MatchingText").GetComponent<Text>().text = matchingText;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Single);
    }
    private void Update()
    {
        //マッチング中
        if (matching)
        {
            string maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
            string playerCount = PhotonNetwork.PlayerList.Length.ToString();
            matchingBackPanel.transform.Find("MatchingMaxPlayersText").GetComponent<Text>().text = maxPlayers;
            matchingBackPanel.transform.Find("MatchingPlayerCountText").GetComponent<Text>().text = playerCount;
            if (!popupBackPanel.activeSelf)
            {
                if (!matchingBackPanel.activeSelf)
                {
                    matchingBackPanel.SetActive(true);
                }
            }
            if (playerCount == maxPlayers)
            {
                if (popupBackPanel.activeSelf)
                {
                    popupBackPanel.SetActive(false);
                }
                if (!matchingBackPanel.activeSelf)
                {
                    matchingBackPanel.SetActive(true);
                }
                MatchingCancelButton.interactable = false;
                matching = false;
                StartCoroutine(MatchingComplete());
            }
        }
    }
    //設定ボタン押下
    public void MoveCustomScene()
    {
        loadingPanel.SetActive(true);
        serverConnecter.DisconnectServer();
        StartCoroutine(MoveCustomSceneColutine());
    }
    IEnumerator MoveCustomSceneColutine()
    {
        bool playingSound = true;
        audioSource.Play();
        while (playingSound)
        {
            if (!audioSource.isPlaying)
            {
                playingSound = false;
                SceneManager.LoadScene("CustomScene", LoadSceneMode.Single);
            }
            yield return null;
        }
    }
    //キャラクターセレクトボタン押下
    public void MoveCharacterSlect()
    {
        loadingPanel.SetActive(true);
        serverConnecter.DisconnectServer();
        StartCoroutine(MoveCharacterSelectColutine());
    }
    IEnumerator MoveCharacterSelectColutine()
    {
        bool playingSound = true;
        audioSource.Play();
        while (playingSound)
        {
            if (!audioSource.isPlaying)
            {
                playingSound = false;
                SceneManager.LoadScene("CharacterSelect", LoadSceneMode.Single);
            }
            yield return null;
        }
    }
    //マッチングキャンセル
    public void MatchingCancel()
    {
        matching = false;
        audioSource.Play();
        if (matchingBackPanel.activeSelf)
        {
            matchingBackPanel.SetActive(false);
        }
        PhotonNetwork.LeaveRoom();  //ルーム退出
        PopupMessage("キャンセルしました。");
        UpdateRoom();
    }
    //ロビー入室時
    public override void OnJoinedLobby()
    {
        UpdateRoom();
    }
}
