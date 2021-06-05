using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum MagicType
{
    Water,
    Wind
}

public class Player : Character
{
    public static Player Instance = null;
    public Vector3 Pos
    {
        get { return Instance.transform.position; }
        private set { }
    }

    private MagicType _curMagic = MagicType.Wind;
    public MagicType CurMagic
    {
        get { return _curMagic; }
        set
        {
            _curMagic = value;
            OnChangedMagic?.Invoke();
        }
    }
    public UnityEvent OnChangedMagic = new UnityEvent();

    [Header("Movement")]
    [SerializeField] private Transform _cam = null;
    [SerializeField] private float _speed = 6f;
    [SerializeField] private float _turnSmoothTime = 0.1f;
    [SerializeField] private float _slopeForce = 25f;
    [SerializeField] private float _slopeForceRayLength = 1f;

    [Header("Interactive")]
    [SerializeField] private float _dectRayLength = 5.0f;
    [SerializeField] private Vector3 _dectBoxHalfExtent = new Vector3(0.3f, 0.3f, 0.3f);

    [Header("???")]
    [SerializeField] private GameObject _itemInHand = null;

    private CharacterController _charController = null;
    private float _turnSmoothVelocity;
    private Animator _anim;
    private IEnumerable<GameObject> _hitObjs;
    private InteractiveableMark _markComp = null;
    private AudioSource _submitSFX = null;

    /////////////////////////////////////////////////////
    /////////////////////////////////////////////////////
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;

        _markComp = GetComponent<InteractiveableMark>();
        _charController = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        _submitSFX = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StoryManager.OnSlovedRiverEvent.AddListener(() =>
        {
            _markComp.CurType = InteractiveType.None;
        });
    }

    private void Update()
    {
        // About Sences
        var _hitInfos = Physics.BoxCastAll(transform.position, _dectBoxHalfExtent,
            transform.forward, Quaternion.identity, _dectRayLength);
        _hitObjs = from o in _hitInfos select o.collider.gameObject;

        if (_hitObjs.Any(o => o.CompareTag("NPC")))
        {
            _markComp.CurType = InteractiveType.Talkable;
        }
        else if (_hitObjs.Any(o => o.CompareTag("House") || o.CompareTag("Wind") ||
            o.CompareTag("River")))
        {
            _markComp.CurType = InteractiveType.Magicable;
        }
        else
        {
            _markComp.CurType = InteractiveType.None;
        }

        // About animatior
        if (_charController.velocity.magnitude >= 0.001f)
        {
            _anim.SetBool("isMoving", true);
            var footDust = this.transform.GetChild(2).GetComponent<ParticleSystem>();
            if (footDust.isStopped) footDust.Play();
        }
        else
        {
            _anim.SetBool("isMoving", false);
            var footDust = this.transform.GetChild(2).GetComponent<ParticleSystem>();
            footDust.Stop();
        }

        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Hero@TakeOut"))
        {
            // TODO: Change item sprite by holding one
            _itemInHand.SetActive(true);
        }
        else
        {
            _itemInHand.SetActive(false);
        }

        // About bool check
        bool isChating = DialogueBoxManager.Instance.IsChating();

        // Dont process input logic
        if (StoryManager.IsProcessingEvent || InventoryManager.Instance.IsOpening())
        {
            return;
        }

        #region Input
        if (!isChating)
        {
            HandleMovement();
        }

        if (Input.GetButtonDown("Submit"))
        {
            Interactive();
            _submitSFX.Play();
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            CastMagic();
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            InventoryManager.Instance.Open();
        }
        else if (GHSUtility.GetCrossKeyDown())
        {
            switch (CurMagic)
            {
                case MagicType.Water:
                    CurMagic = MagicType.Wind;
                    break;
                case MagicType.Wind:
                    CurMagic = MagicType.Water;
                    break;
            }
        }
        #endregion
    }

    private void HandleMovement()
    {
        Vector3 moveDir = Vector3.zero;

        /// Handle input movment
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) *
                Mathf.Rad2Deg + _cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y
                , targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            moveDir = moveDir.normalized * _speed;

            /// Handle slope
            if (OnSlop())
            {
                moveDir += Vector3.down * _charController.height /
                    2f * _slopeForce;
            }
        }

        /// Handle gravity
        if (!_charController.isGrounded)
        {
            moveDir += Physics.gravity;
        }

        /// Finally apply movement and apply animation
        _charController.Move(moveDir * Time.deltaTime);
    }
    private bool OnSlop()
    {
        RaycastHit hitInfo;
        bool isHit = Physics.Raycast(transform.position, Vector3.down, out hitInfo,
            _charController.height / 2 * _slopeForceRayLength);

        if (isHit && hitInfo.normal != Vector3.up)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void Interactive()
    {
        foreach (var tarObj in _hitObjs)
        {
            if (tarObj.CompareTag("NPC"))
            {
                ITalkable talker = tarObj.GetComponent<ITalkable>();
                GHSUtility.CheckNull(talker);
                talker.Talk();
            }
            else if (tarObj.CompareTag("Item"))
            {
                var newItem = tarObj.GetComponent<Item>().ItemInfo;
                bool isFull = !InventoryManager.Instance.AddItem(newItem);
                if (isFull)
                {
                    Debug.Log("Bag is full");
                }
            }
        }
    }

    private void CastMagic()
    {
        foreach (var tarObj in _hitObjs)
        {
            if (tarObj.CompareTag("House"))
            {
                if (StoryManager.IsGetHouseMission &&
                    !StoryManager.IsHouseMissionFinished)
                {
                    if (CurMagic == MagicType.Water)
                    {
                        _anim.SetTrigger("TakeOut");
                        tarObj.GetComponent<HouseEventTrigger>().BeginHouseEvent();
                    }
                    else
                    {
                        _anim.SetTrigger("No");
                    }
                }
            }
            else if (tarObj.CompareTag("River"))
            {
                if (StoryManager.IsGetRiverMission &&
                    !StoryManager.IsRiverEventFinished)
                {
                    if (CurMagic == MagicType.Water)
                    {
                        _anim.SetTrigger("TakeOut");
                        tarObj.GetComponent<RiverEventTrigger>().BeginRiverEvent();
                    }
                    else
                    {
                        _anim.SetTrigger("No");
                    }
                }
            }
            else if (tarObj.CompareTag("Wind"))
            {
                if (StoryManager.IsGetWindMission &&
                    !StoryManager.IsWindMissionFinished)
                {
                    if (CurMagic == MagicType.Wind)
                    {
                        _anim.SetTrigger("TakeOut");
                        tarObj.GetComponent<WindEventTrigger>().BeginWindEvent();
                    }
                    else
                    {
                        _anim.SetTrigger("No");
                    }
                }
            }
        }
    }
}