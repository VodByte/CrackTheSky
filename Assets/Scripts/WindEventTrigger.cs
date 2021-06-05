using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WindEventTrigger : MonoBehaviour
{
    [SerializeField]
    private Cow _cow = null;
    [SerializeField]
    private GameObject _windMagic = null;
    [SerializeField]
    private GameObject _fireInStove = null;
    [SerializeField]
    private GameObject _kajiya = null;

    private AudioSource[] sounds = null;

    private void Awake()
    {
        GetComponent<BoxCollider>().enabled = false;
        sounds = GetComponents<AudioSource>();

        var pars = _fireInStove.GetComponentsInChildren<ParticleSystem>();
        foreach (var p in pars)
        {
            p.Stop();
        }
        _fireInStove.SetActive(false);
    }

    private void Start()
    {
        StoryManager.OnGetWindMission.AddListener(() => 
            GetComponent<BoxCollider>().enabled = true);
    }

    public void BeginWindEvent()
    {
        GetComponent<BoxCollider>().enabled = false;
        StoryManager.IsWindEventBegined = true;
        StartCoroutine(CastWind());
    }

    private IEnumerator CastWind()
    {
        yield return new WaitForSeconds(0.2f);  // Waif for player finish anim
        _windMagic.transform.position = Player.Instance.Pos;
        _windMagic.transform.rotation = Quaternion.LookRotation(Player.Instance.Pos -
            _kajiya.transform.position, Vector3.up) * Quaternion.Euler(0,90,0);
        _windMagic.SetActive(true);

        sounds.First(o => o.clip.name == "WindMagic").Play();
        _kajiya.GetComponent<Animator>().SetTrigger("OnWind");

        yield return new WaitForSeconds(1.1f); // Ready for fire
        _fireInStove.SetActive(true);
        sounds.First(o => o.clip.name == "StoveBurnFire").Play();
        var pars = _fireInStove.GetComponentsInChildren<ParticleSystem>();
        foreach (var p in pars)
        {
            p.Play();
        }
        _cow.Surprise();
        StoryManager.IsWindMissionFinished = true;
    }
}