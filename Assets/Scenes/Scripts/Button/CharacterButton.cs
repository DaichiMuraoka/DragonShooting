using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterButton : MonoBehaviour
{
    private PlayerController playerController = null;

    public void SetPlayerController(PlayerController _playerController)
    {
        playerController = _playerController;
    }
    public PlayerController GetPlayerController()
    {
        return playerController;
    }
    public void Onclick()
    {
        if(playerController == null)
        {
            Debug.Log("playerData is Null");
            return;
        }
        CharacterSelectManager characterSelectManager = GameObject.Find("CharacterSelectManager").GetComponent<CharacterSelectManager>();
        characterSelectManager.ViewCharacterStatusPanel(playerController);
    }
}
