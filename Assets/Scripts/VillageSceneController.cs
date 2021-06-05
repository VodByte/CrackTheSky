using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageSceneController : MonoBehaviour
{
    private void Start()
    {
        LevelTransition.Instance.WipeScreen();
    }
}