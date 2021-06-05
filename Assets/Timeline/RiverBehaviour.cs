using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

// A behaviour that is attached to a playable
public class RiverBehaviour : PlayableBehaviour
{
    private CinemachineVirtualCamera cam;

    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        var camInterface = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;
        cam = camInterface.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        StoryManager.IsRiverEventFinished = true;
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        cam.GetComponent<CameraShake>().m_Range = 0.12f;
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (cam)
        {
            cam.GetComponent<CameraShake>().m_Range = 0.0f;
        }
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
    }
}