using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;

// A behaviour that is attached to a playable
public class GoatHouseBehaviour : PlayableBehaviour
{
    private float _timer = 0.0f;
    private DialogueBoxManager _dm;
    public GameObject Goat;
    public GameObject House;
    public GameObject HouseTrigger;
    private float _waitTime = 2.3f;

    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        // 跑回房子的路上不能被对话
        Goat.GetComponent<Goat>().CanTalk = false;
        StoryManager.IsProcessingGoatEvent = true;
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        if (_dm)
        {
            _dm.CloseDiaolugeBox();
        }
        StoryManager.IsProcessingGoatEvent = false;
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        Goat.GetComponent<Goat>().CanTalk = true;
        _timer = 0.0f;
        _dm = DialogueBoxManager.Instance;
        if (_dm)
        {
            _dm.UpdateText("<b>燃え上がる屋敷：</b><size=120%><b><i>＃!",
            House.transform.position);
        }
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        _timer += info.deltaTime;
        if (_timer >= _waitTime)
        {
            var clip = HouseTrigger.GetComponents<AudioSource>().
                First(o => o.clip.name == "BurningHouse_Again");
            if (!clip.isPlaying) clip.Play();

            _dm.UpdateText("<b>羊：</b>土足で上がっただけなのにぃぃぃぃ！",
                Goat.transform.position/*, false*/);
        }
    }
}