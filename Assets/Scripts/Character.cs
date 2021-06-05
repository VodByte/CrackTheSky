using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Character : MonoBehaviour
{
    [SerializeField]
    private string _characterName;
    public string CharacterName 
    {
        get { return _characterName; }
        private set { _characterName = value; }
    }

    private void Awake()
    {
        if (string.IsNullOrEmpty(CharacterName))
        {
            Debug.LogError($"Character {this.gameObject.name}'s name not be setted!");
        }
    }
}