using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField]
    private SaveDataManager saveDataManager;
    [SerializeField]
    private GameObject popupBackPanel;
    [SerializeField]
    private AudioClip startSound;
    private AudioSource audioSource;
    private bool starting = false;
    [SerializeField]
    private GameObject loadingPanel;
    [SerializeField]
    private BGMManager bgmManager;
    private void Start()
    {
        loadingPanel.SetActive(true);
        saveDataManager.Load();
        loadingPanel.SetActive(false);
        PlayerDataAsset playerDataAsset = saveDataManager.GetPlayerDataAsset();
        bgmManager.SetBGMVolume(playerDataAsset.GetBGMVolume());
        bgmManager.RandomPlay();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = playerDataAsset.GetSEVolume();
        audioSource.clip = startSound;
    }
    private void Update()
    {
        if (popupBackPanel.activeSelf)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) && !starting)
        {
            audioSource.Play();

            starting = true;
        }
        if(starting && !audioSource.isPlaying)
        {
            starting = false;
            SceneManager.LoadScene("ModeSelect", LoadSceneMode.Single);
        }
    }
}
