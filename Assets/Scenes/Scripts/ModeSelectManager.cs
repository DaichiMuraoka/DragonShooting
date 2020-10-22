using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModeSelectManager : MonoBehaviour
{
    [SerializeField]
    private GameObject popupBackPanel;
    [SerializeField]
    private GameObject LoadingPanel;
    [SerializeField]
    private SaveDataManager saveDataManager;
    [SerializeField]
    private AudioClip clickSound;
    private AudioSource audioSource;
    [SerializeField]
    private Button soloPlayButton;
    [SerializeField]
    private BGMManager bgmManager;

    private bool changingScene = false;

    private void Start()
    {
        soloPlayButton.interactable = false;
        audioSource = GetComponent<AudioSource>();
        PlayerDataAsset playerDataAsset = saveDataManager.GetPlayerDataAsset();
        audioSource.volume = playerDataAsset.GetSEVolume();
        audioSource.clip = clickSound;
        bgmManager.SetBGMVolume(playerDataAsset.GetBGMVolume());
        bgmManager.RandomPlay();

    }
    private void Update()
    {
        if (changingScene && !audioSource.isPlaying)
        {
            changingScene = false;
            SceneManager.LoadScene("RoomLobby", LoadSceneMode.Single);
        }
    }

    public void GoToLobby()
    {
        audioSource.Play();
        LoadingPanel.SetActive(true);
        changingScene = true;
    }
}
