using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityButton : MonoBehaviour
{
    private BulletController ability = null;
    private int number = 0;
    public void SetAbility(BulletController _ability, int _number)
    {
        ability = _ability;
        number = _number;
    }
    public BulletController GetAbility()
    {
        return ability;
    }
    public void OnClick()
    {
        if (ability == null)
        {
            return;
        }
        GameObject SetIcon = transform.Find("SetIcon").gameObject;
        if (SetIcon.activeSelf)
        {
            return;
        }
        SetIcon.SetActive(true);
        CharacterSelectManager characterSelectManager = GameObject.Find("CharacterSelectManager").GetComponent<CharacterSelectManager>();
        characterSelectManager.SetAbility(ability, number);
    }
}
