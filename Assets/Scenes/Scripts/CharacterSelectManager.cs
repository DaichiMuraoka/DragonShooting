using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour
{
    [SerializeField]
    private GameObject characterButtonContent;
    [SerializeField]
    private GameObject characterButtonPrefab;
    [SerializeField]
    private GameObject characterStatusPanel;
    //アビリティ変更
    [SerializeField]
    private GameObject abilitySelectBackPanel;
    [SerializeField]
    private GameObject abilityContent;
    [SerializeField]
    private GameObject abilityButtonPrefab;
    //
    private PlayerController viewingCharacter = null;
    [SerializeField]
    private SaveDataManager saveDataManager;
    [SerializeField]
    private List<PlayerController> allCharacters;
    [SerializeField]
    private List<BulletController> allAbilities;
    [SerializeField]
    private AudioClip clickSound;
    private AudioSource audioSource;
    [SerializeField]
    private GameObject loadingPanel;
    [SerializeField]
    private BGMManager bgmManager;
    void Start()
    {
        loadingPanel.SetActive(true);
        characterStatusPanel.SetActive(false);
        abilitySelectBackPanel.SetActive(false);
        //CreateCharacterButtons();
        LoadCharacterData();
        PlayerDataAsset playerDataAsset = saveDataManager.GetPlayerDataAsset();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = playerDataAsset.GetSEVolume();
        audioSource.clip = clickSound;
        loadingPanel.SetActive(false);
        bgmManager.SetBGMVolume(playerDataAsset.GetBGMVolume());
        bgmManager.RandomPlay();
    }
    void Update()
    {
        
    }
    public void LoadCharacterData()
    {
        PlayerDataAsset playerDataAsset = saveDataManager.GetPlayerDataAsset();
        string characterDatas = playerDataAsset.GetCharacterData();
        List<string> characterDataList = new List<string>();
        if (characterDatas.Contains("/"))
        {
            characterDataList.AddRange(characterDatas.Split('/'));
        }
        else
        {
            characterDataList.Add(characterDatas);
        }
        foreach(string characterData in characterDataList)
        {
            Debug.Log(characterData);
            List<string> dataList = new List<string>();
            dataList.AddRange(characterData.Split(':'));
            foreach (PlayerController playerController in allCharacters)
            {
                if(dataList[0] == playerController.name)
                {
                    playerController.ResetAbility();
                    int i = 0;
                    foreach (string data in dataList)
                    {
                        foreach (BulletController ability in allAbilities)
                        {
                            if (data == ability.name)
                            {
                                Debug.Log(data);
                                playerController.SetAbility(ability, Mathf.Min(i, 3));
                            }
                        }
                        i++;
                    }
                    GameObject characterButton = Instantiate(characterButtonPrefab, characterButtonContent.transform);
                    characterButton.transform.Find("CharacterNameText").GetComponent<Text>().text = playerController.name;
                    characterButton.GetComponent<CharacterButton>().SetPlayerController(playerController);
                }
            }
        }
    }
    public void ViewCharacterStatusPanel(PlayerController playerController)
    {
        audioSource.Play();
        characterStatusPanel.SetActive(true);
        viewingCharacter = playerController;
        characterStatusPanel.transform.Find("CharacterNameText").GetComponent<Text>().text = playerController.name;
        BulletController LeftAbility = playerController.GetAbility(1);
        if(LeftAbility != null)
        {
            characterStatusPanel.transform.Find("LeftAbilityPanel").transform.Find("AbilityNameText").GetComponent<Text>().text = LeftAbility.name;
        }
        BulletController RightAbility = playerController.GetAbility(2);
        if (RightAbility != null)
        {
            characterStatusPanel.transform.Find("RightAbilityPanel").transform.Find("AbilityNameText").GetComponent<Text>().text = RightAbility.name;
        }
        PlayerData playerData = saveDataManager.GetPlayerData();
        playerData.SetBattleCharacter(playerController.name);
        saveDataManager.SetPlayerData(playerData);
        saveDataManager.Save();
    }
    public void AbilitySelect(int number)
    {
        audioSource.Play();
        abilitySelectBackPanel.SetActive(true);
        BulletController settingAbility = viewingCharacter.GetAbility(number);
        if(settingAbility != null)
        {
            CreateAbilityButton(settingAbility, true, number);
        }
        List<BulletController> otherAbilities = viewingCharacter.GetOtherAbilities();
        if(otherAbilities != null)
        {
            foreach(BulletController ability in otherAbilities)
            {
                CreateAbilityButton(ability, false, number);
            }
        }
    }
    public void CreateAbilityButton(BulletController ability, bool displayIcon, int number)
    {
        GameObject abilityButton = Instantiate(abilityButtonPrefab, abilityContent.transform);
        abilityButton.transform.Find("SetIcon").gameObject.SetActive(displayIcon);
        abilityButton.transform.Find("AbilityNameText").GetComponent<Text>().text = ability.name;
        abilityButton.transform.Find("SpeedText").GetComponent<Text>().text = ability.GetSpeed().ToString();
        abilityButton.transform.Find("PowerText").GetComponent<Text>().text = ability.GetPower().ToString();
        abilityButton.transform.Find("CoolTimeText").GetComponent<Text>().text = ability.GetCoolTime().ToString();
        abilityButton.GetComponent<AbilityButton>().SetAbility(ability, number);
    }
    public void SetAbility(BulletController ability, int number)
    {
        audioSource.Play();
        BulletController removeAbility = viewingCharacter.GetAbility(number);
        if (removeAbility != null)
        {
            viewingCharacter.SetAbility(removeAbility, 3);
            foreach (Transform button in abilityContent.transform)
            {
                if (button.gameObject.GetComponent<AbilityButton>().GetAbility().name == removeAbility.name)
                {
                    GameObject SetIcon = button.transform.Find("SetIcon").gameObject;
                    SetIcon.SetActive(false);
                }
            }
        }
        viewingCharacter.SetAbility(ability, number);
        viewingCharacter.RemoveAbility(ability.name);
        SaveCharacterData();
    }
    public void HideAbilityPanel()
    {
        abilitySelectBackPanel.SetActive(false);
        foreach (Transform abilityButtonTransform in abilityContent.transform)
        {
            Destroy(abilityButtonTransform.gameObject);
        }
        ViewCharacterStatusPanel(viewingCharacter);
    }
    public void BackRoomLobby()
    {
        loadingPanel.SetActive(true);
        StartCoroutine(BackRoomLobbyColutine());
    }
    IEnumerator BackRoomLobbyColutine()
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
    public void SaveCharacterData()
    {
        string characterData = "";
        int i = 0;
        foreach(PlayerController character in allCharacters)
        {
            if(i > 0)
            {
                characterData += "/";
            }
            string data = character.GetStringData();
            characterData += data;
            i += 1;
        }
        PlayerData playerData = saveDataManager.GetPlayerData();
        playerData.SetCharacterData(characterData);
        saveDataManager.SetPlayerData(playerData);
        saveDataManager.Save();
    }
}
