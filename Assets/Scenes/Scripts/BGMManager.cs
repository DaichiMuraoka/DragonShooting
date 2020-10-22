using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> bgmList;
    private AudioSource audioSource;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void RandomPlay()
    {
        audioSource = GetComponent<AudioSource>();
        int rdm = Random.Range(0, bgmList.Count);
        audioSource.clip = bgmList[rdm];
        audioSource.Play();
    }
    public void SetBGMVolume(float BGMVolume)
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = BGMVolume;
    }
}
