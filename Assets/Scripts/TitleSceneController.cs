using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class TitleSceneController : MonoBehaviour
{
    public GameObject ParticlePerfab;
    [SerializeField] VideoPlayer _player = null;
    [SerializeField] float _demoWaitTime = 11f;
    private float _demoTimer = 0.0f;

    private GameObject _parInstance;
    private bool pushAnyKey = false;

    private float rotX;
    private float rotY;
    private float speed;
    private float timeT;

    void Start()
    {
        speed = 200.0f;
        _parInstance = Instantiate(ParticlePerfab, new Vector3(2.0f, 2.4f, 0.0f), Quaternion.identity);

        StoryManager.Reset();
    }

    void Update()
    {
        if (!_player.isPlaying)
        {
            _demoTimer += Time.deltaTime;
            if (_demoTimer >= _demoWaitTime)
            {
                _demoTimer = 0.0f;
                _player.gameObject.SetActive(true);
                _player.Play();
            }

            if (GHSUtility.GetAnyGamepadKey() && !pushAnyKey)
            {
                pushAnyKey = true;

                var sounds = GetComponents<AudioSource>();
                sounds.Where(x => x.clip.name == "TitleSubmit").First().Play();

                var transition = LevelTransition.Instance;
                if (transition)
                {
                    transition.WipeScreen(new System.Action(() =>
                        SceneManager.LoadScene(1)));
                }
            }
        }
        else
        {
            if (GHSUtility.GetAnyGamepadKey())
            {
                _player.gameObject.SetActive(false);
                _player.Stop();
            }
        }

        if (pushAnyKey)
        {
            _parInstance.transform.position += 
                (transform.position - _parInstance.transform.position).normalized * 2.0f * Time.deltaTime;
        }
        else
        {
            timeT = Time.deltaTime;
            rotX += Mathf.PI * 2.0f / 360.0f;
            rotY += Mathf.PI * 2.0f / 360.0f;

            _parInstance.transform.position = new Vector3(4.0f + timeT * speed * 3.1f * Mathf.Cos(rotX), 
                2.4f + timeT * speed * Mathf.Sin(rotY), 
                timeT * speed * Mathf.Cos(rotY));
        }
    }
}