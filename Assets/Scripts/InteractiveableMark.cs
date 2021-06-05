using UnityEngine;

public enum InteractiveType
{
    Talkable,
    Magicable,
    Missionable,
    None
}

public class InteractiveableMark : MonoBehaviour
{
    [SerializeField]
    private Vector3 PosBias = new Vector3();

    private InteractiveType _curType = InteractiveType.None;
    public InteractiveType CurType
    {
        get { return _curType; }
        set
        {
            if (_curType == value) return;
            if (_curUI != null || value == InteractiveType.None) Destroy(_curUI);
            _curType = value;
            switch (_curType)
            {
                case InteractiveType.Talkable:
                    _curUI = Instantiate(_talkableUIPerfab, new Vector3(100000, 100000, 100000),
                        Quaternion.identity, _markUIRoot) as GameObject;
                    break;
                case InteractiveType.Magicable:
                    if (!IsDirtyMark())
                    {
                        _curUI = Instantiate(_magicableUIPerfab, new Vector3(100000, 100000, 100000),
                            Quaternion.identity, _markUIRoot) as GameObject;
                    }
                    break;
                case InteractiveType.Missionable:
                    _curUI = Instantiate(_missionableUIPerfab, new Vector3(100000, 100000, 100000),
                            Quaternion.identity, _markUIRoot) as GameObject;
                    break;
            }
        }
    }

    [SerializeField]
    private RectTransform _markUIRoot = null;
    [SerializeField]
    private float _detecRange = 2f;

    private Object _talkableUIPerfab = null;
    private Object _magicableUIPerfab = null;
    private Object _missionableUIPerfab = null;
    private GameObject _curUI = null;

    public void Awake()
    {
        CurType = InteractiveType.None;
        _talkableUIPerfab = GHSUtility.LoadPrefabFromFile("Image_Talkable");
        _magicableUIPerfab = GHSUtility.LoadPrefabFromFile("Image_Magicable");
        _missionableUIPerfab = GHSUtility.LoadPrefabFromFile("Image_Missionable");
    }

    public void Update()
    {
        if (CurType == InteractiveType.None || _curUI == null)
        {
            return;
        }

        if (Vector3.Distance(Player.Instance.Pos, this.transform.position) <= _detecRange
            && !DialogueBoxManager.Instance.IsChating())
        {
            if (!_curUI.activeSelf) _curUI.SetActive(true);
            _curUI.transform.position = GHSUtility.WorldToViewportPoint(this.transform.position +
                PosBias);
        }
        else
        {
            _curUI.SetActive(false);
        }
    }

    // 只有在跟狗讲话之后的mark是固定在河道的，所以主角头上的mark就叫dirty mark
    private bool IsDirtyMark()
    {
        return this.gameObject.CompareTag("Player") && StoryManager.IsGetRiverMission
                        && !StoryManager.IsRiverEventFinished;
    }
}