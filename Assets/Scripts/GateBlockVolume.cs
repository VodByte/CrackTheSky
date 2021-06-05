using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GateBlockVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelTransition.Instance.WipeScreen(new System.Action(() =>
                    SceneManager.LoadScene(2)));
        }       
    }
}