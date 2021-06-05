using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Debuger : MonoBehaviour
{
    void Update()
    {
        // Reload this scene
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StoryManager.Reset();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            StoryManager.IsHouseMissionFinished = true;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            StoryManager.IsGetForestLincense = true;
        }
    }
}