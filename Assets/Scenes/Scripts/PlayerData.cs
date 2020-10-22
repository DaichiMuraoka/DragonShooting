using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    private string playersName = "player";
    private float bgmVolume = 0.2f;
    private float seVolume = 0.2f;
    private string characterData = "NomalDragon:FireBall:FlameThrower:EnagyBall/MegaDraco:FireBall:FlameThrower:EnagyBall";
    private string battleCharacter = "NomalDragon";
    private string itemData = "EnagyClowd:1";
    private int winCount = 0;
    public string GetPlayersName()
    {
        return playersName;
    }
    public void SetPlayersName(string newPlayersName)
    {
        playersName = newPlayersName;
    }
    public float GetBGMVolume()
    {
        return bgmVolume;
    }
    public void SetBGMVolume(float newBGMVolume)
    {
        bgmVolume = newBGMVolume;
    }
    public float GetSEVolume()
    {
        return seVolume;
    }
    public void SetSEVolume(float newSEVolume)
    {
        seVolume = newSEVolume;
    }
    public string GetCharacterData()
    {
        return characterData;
    }
    public void SetCharacterData(string newCharacterData)
    {
        characterData = newCharacterData;
    }
    public string GetItemData()
    {
        return itemData;
    }
    public void SetItemData(string newItemData)
    {
        itemData = newItemData;
    }
    public void IncrementWinCount()
    {
        winCount++;
    }
    public void SetWinCount(int newWinCount)
    {
        winCount = newWinCount;
    }
    public int GetWinCount()
    {
        return winCount;
    }
    public string GetBattleCharacter()
    {
        return battleCharacter;
    }
    public void SetBattleCharacter(string newCharacter)
    {
        battleCharacter = newCharacter;
    }
}
