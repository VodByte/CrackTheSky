using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class RiverEventTrigger : MonoBehaviour
{
    private InteractiveableMark _mark;

    private void Awake()
    {
        GetComponent<BoxCollider>().enabled = false;
        _mark = GetComponent<InteractiveableMark>();
    }

    private void Start()
    {
        StoryManager.OnGetRiverMission.AddListener(() =>
        {
            GetComponent<BoxCollider>().enabled = true;
            _mark.CurType = InteractiveType.Magicable;
        });

        StoryManager.OnSlovedRiverEvent.AddListener(() => 
        { 
            var markComp = GetComponent<InteractiveableMark>();
        });
    }

    public void BeginRiverEvent()
    {
        GetComponent<AudioSource>().Play();
        _mark.CurType = InteractiveType.None;
        GetComponent<BoxCollider>().enabled = false;
        StoryManager.IsRiverEventBegined = true;
        GetComponent<PlayableDirector>().Play();
    }
}