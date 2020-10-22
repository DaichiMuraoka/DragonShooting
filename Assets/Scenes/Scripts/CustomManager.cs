using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomManager : MonoBehaviour
{
    [SerializeField]
    private InputField nameInputField;
    //BGM
    [SerializeField]
    private Slider BGMVolumeSlider;
    [SerializeField]
    private Text BGMValueText;
    private float BGMVolume = 0.2f;
    //SE
    [SerializeField]
    private Slider SEVolumeSlider;
    [SerializeField]
    private Text SEValueText;
    private float SEVolume = 0.2f;
    [SerializeField]
    private SaveDataManager saveDataManager;
    [SerializeField]
    private GameObject popupBackPanel;
    [SerializeField]
    private AudioClip clickSound;
    private AudioSource audioSource;
    [SerializeField]
    private GameObject loadingPanel;
    [SerializeField]
    private BGMManager bgmManager;
    private void Start()
    {
        loadingPanel.SetActive(true);
        PlayerDataAsset playerDataAsset = saveDataManager.GetPlayerDataAsset();
        nameInputField.text = playerDataAsset.GetPlayersName();
        //BGM
        BGMVolumeSlider.value = playerDataAsset.GetBGMVolume();
        BGMVolume = BGMVolumeSlider.value;
        BGMValueText.text = (BGMVolume * 100).ToString();
        //SE
        SEVolumeSlider.value = playerDataAsset.GetSEVolume();
        SEVolume = SEVolumeSlider.value;
        SEValueText.text = (SEVolume * 100).ToString();
        //タップ音
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = playerDataAsset.GetSEVolume();
        audioSource.clip = clickSound;
        loadingPanel.SetActive(false);
        bgmManager.SetBGMVolume(BGMVolume);
        bgmManager.RandomPlay();
    }
    private void Update()
    {
        
    }
    public void ChangeBGMVolume()
    {
        BGMVolume = Mathf.Floor(BGMVolumeSlider.value * 100) / 100;
        bgmManager.SetBGMVolume(BGMVolume);
        BGMValueText.text = (BGMVolume * 100).ToString();
    }
    public void ChangeSEVolume()
    {
        SEVolume = Mathf.Floor(SEVolumeSlider.value * 100) / 100;
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        audioSource.volume = SEVolume;
        SEValueText.text = (SEVolume * 100).ToString();
    }
    public void CustomUpdate()
    {
        audioSource.Play();
        PlayerData playerData = saveDataManager.GetPlayerData();
        playerData.SetPlayersName(nameInputField.text);
        playerData.SetBGMVolume(BGMVolume);
        playerData.SetSEVolume(SEVolume);
        saveDataManager.SetPlayerData(playerData);
        saveDataManager.Save();
        PopupMessage("反映しました");
    }
    public void MoveRoomLobby()
    {
        loadingPanel.SetActive(true);
        StartCoroutine(MoveLoomLobbyColutine());
    }
    IEnumerator MoveLoomLobbyColutine()
    {
        bool playingSound = true;
        audioSource.Play();
        while (playingSound)
        {
            if (!audioSource.isPlaying)
            {
                playingSound = false;
                SceneManager.LoadScene("RoomLobby", LoadSceneMode.Single);
            }
            yield return null;
        }
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
