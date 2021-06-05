using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HouseEventTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject _fireLight = null;
    [SerializeField]
    private Transform _rainFXRoot = null;
    [SerializeField]
    private Transform _SmokeFXRoot = null;
    [SerializeField]
    private Transform _fireFXRoot = null;
    [SerializeField]
    private float _fireDownTime = 0.7f;
    [SerializeField]
    private float _rainOverTime = 1.2f;

    private AudioSource[] sounds = null;

    private void Awake()
    {
        GetComponent<BoxCollider>().enabled = false;
        sounds = GetComponents<AudioSource>();
    }

    private void Start()
    {
        StoryManager.OnGetHouseMission.AddListener(() =>
        {
            //var markComp = this.GetComponent<InteractiveableMark>();
            //markComp.enabled = true;
            //markComp.CurType = InteractiveType.Magicable;
            GetComponent<BoxCollider>().enabled = true;
        });

        //StoryManager.OnSlovedHouseMission.AddListener(() =>
        //{
        //});
    }

    public void BeginHouseEvent()
    {
        GetComponent<BoxCollider>().enabled = false;
        sounds.First(o => o.clip.name == "BurningHouse").Stop();
        sounds.First(o => o.clip.name == "WaterMagic").Play();
        StoryManager.IsHouseMissionBegined = true;
        _rainFXRoot.gameObject.SetActive(true);
        StartCoroutine(DownFire());
    }

    private IEnumerator DownFire()
    {
        yield return new WaitForSeconds(_fireDownTime);
        var fireParSys = _fireFXRoot.GetComponentsInChildren<
           ParticleSystem>();
        foreach (var p in fireParSys)
        {
            p.Stop();
        }
        _fireLight.SetActive(false);
        _SmokeFXRoot.gameObject.SetActive(true);
        // Close rain and cloud
        yield return new WaitForSeconds(_rainOverTime);
        var rainParSys = _rainFXRoot.gameObject.GetComponentsInChildren<
            ParticleSystem>();
        foreach (var p in rainParSys)
        {
            p.Stop();
        }
        // Trigger Event
        StoryManager.IsHouseMissionFinished = true;
    }
}