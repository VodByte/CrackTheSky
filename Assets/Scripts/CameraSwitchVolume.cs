using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(BoxCollider))]
public class CameraSwitchVolume : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _soloVCam = null;

    public void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _soloVCam.gameObject.SetActive(true);
            _soloVCam.MoveToTopOfPrioritySubqueue();
        }
    }
}