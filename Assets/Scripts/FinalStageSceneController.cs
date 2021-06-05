using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class FinalStageSceneController : MonoBehaviour
{
    private bool _isFinish = false;
    private void Start()
    {
        GetComponent<PlayableDirector>().stopped += x => _isFinish = true;

        LevelTransition.Instance.WipeScreen(new System.Action(()=>
        { 
            GetComponent<PlayableDirector>().Play();
        }));
    }

    private void Update()
    {
        if (!_isFinish) return;

        if (Input.GetKeyDown(KeyCode.Joystick1Button0) ||
            Input.GetKeyDown(KeyCode.J))
        {
            SceneManager.LoadScene(0);
        }
    }
}