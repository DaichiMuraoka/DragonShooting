using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class SaveDataManager : MonoBehaviour
{
    //ファイルストリーム
    private FileStream fileStream;
    //バイナリフォーマッター
    private BinaryFormatter bf;
    //プレイヤーデータプレファブ
    [SerializeField]
    private PlayerDataAsset playerDataAsset;
    //ポップアップ
    [SerializeField]
    private GameObject popupBackPanel;
    //セーブ
    public void Save()
    {
        if(File.Exists(Application.dataPath + "/playerData.dat") == false)
        {
            PopupMessage("セーブデータがありません。");
        }
        else
        {
            OverwriteSaveData();
        }
    }
    //セーブデータ上書き
    public void OverwriteSaveData()
    {
        bf = new BinaryFormatter();
        fileStream = null;
        try
        {
            fileStream = File.Create(Application.dataPath + "/playerData.dat");
            PlayerData playerData = ConvertFromAsset();
            bf.Serialize(fileStream, playerData);
        }
        finally
        {
            if (fileStream != null)
            {
                fileStream.Close();
            }
        }
    }
    //ロード
    public void Load()
    {
        if (File.Exists(Application.dataPath + "/playerData.dat") == false)
        {
            PopupMessage("セーブデータがありません。新規作成します。");
            CreateNewSaveData();
        }
        else
        {
            PopupMessage("セーブデータを読み込みます。");
            LoadSaveData();
        }
    }
    //新規データ作成
    public void CreateNewSaveData()
    {
        bf = new BinaryFormatter();
        fileStream = null;
        try
        {
            fileStream = File.Create(Application.dataPath + "/playerData.dat");
            PlayerData newPlayerData = new PlayerData();
            bf.Serialize(fileStream, newPlayerData);
        }
        finally
        {
            if (fileStream != null)
            {
                PopupMessage("セーブデータを新規作成しました。");
                fileStream.Close();
            }
        }
    }
    //データ読み込み
    public void LoadSaveData()
    {
        bf = new BinaryFormatter();
        fileStream = null;
        try
        {
            fileStream = File.Open(Application.dataPath + "/playerData.dat", FileMode.Open);
            PlayerData loadPlayerData = bf.Deserialize(fileStream) as PlayerData;
            SetPlayerData(loadPlayerData);
        }
        finally
        {
            if (fileStream != null)
            {
                PopupMessage("セーブデータを読み込みました。");
                fileStream.Close();
            }
        }
    }
    //プレイヤーデータアセットを渡す
    public PlayerDataAsset GetPlayerDataAsset()
    {
        return playerDataAsset;
    }
    public PlayerData GetPlayerData()
    {
        PlayerData data = ConvertFromAsset();
        return data;
    }
    //プレイヤーデータを更新
    public void SetPlayerData(PlayerData newPlayerData)
    {
        if(newPlayerData != null)
        {
            playerDataAsset.SetPlayersName(newPlayerData.GetPlayersName());
            playerDataAsset.SetBGMVolume(newPlayerData.GetBGMVolume());
            playerDataAsset.SetSEVolume(newPlayerData.GetSEVolume());
            playerDataAsset.SetCharacterData(newPlayerData.GetCharacterData());
            playerDataAsset.SetItemData(newPlayerData.GetItemData());
            playerDataAsset.SetWinCount(newPlayerData.GetWinCount());
            playerDataAsset.SetBattleCharacter(newPlayerData.GetBattleCharacter());
        }
        else
        {
            PopupMessage("セーブデータが読み込まれていません");
        }
    }
    //PlayerData型変換
    private PlayerData ConvertFromAsset()
    {
        PlayerData playerData = new PlayerData();
        playerData.SetPlayersName(playerDataAsset.GetPlayersName());
        playerData.SetBGMVolume(playerDataAsset.GetBGMVolume());
        playerData.SetSEVolume(playerDataAsset.GetSEVolume());
        playerData.SetCharacterData(playerDataAsset.GetCharacterData());
        playerData.SetItemData(playerDataAsset.GetItemData());
        playerData.SetWinCount(playerDataAsset.GetWinCount());
        playerData.SetBattleCharacter(playerDataAsset.GetBattleCharacter());
        return playerData;
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

}
