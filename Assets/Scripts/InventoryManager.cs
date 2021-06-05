using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance = null;

    //[SerializeField] private GameObject _itemSlotPerfab = null;
    [SerializeField] private RectTransform _itemSlotParent = null;
    [SerializeField] private int _maxCapacity = 15;
    [SerializeField] private TMP_Text _missionInfoText = null;
    [SerializeField] private TMP_Text _itemInfoText = null;
    [SerializeField] private ItemAsset _lincenseItem = null;
    [SerializeField] private ItemUI _lincenseSlotUI = null;

    private List<GameObject> _itemSlots = new List<GameObject>();
    private int _itemCount = 0;

    // Without this, Close() method will be called at same memonent
    // with Player.cs input method
    private const float _closeableDelay = 0.02f;
    private float _closeableDelayTimer = 0.0f;
    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;

        //for (int i = 0; i < _maxCapacity; i++)
        //{
        //    var newSlot = Instantiate(_itemSlotPerfab, _itemSlotParent);
        //    _itemSlots.Add(newSlot);
        //}

        StoryManager.OnGetForestLincense.AddListener(()=>
        {
            _lincenseSlotUI.ItemInfo = _lincenseItem;
        });
    }

    private void Start()
    {
        StoryManager.OnGetHouseMission.AddListener(() =>
        {
            _missionInfoText.text = "怒ってる家\n" +
            "家の怒り炎をしずめよう！";
        });

        StoryManager.OnSlovedHouseMission.AddListener(() =>
        {
            _missionInfoText.text = "無し";
        });

        StoryManager.OnGetRiverMission.AddListener(() =>
        {
            _missionInfoText.text = "枯れた川\n" +
            "何で水が来ないかな？";
        });

        StoryManager.OnSlovedRiverEvent.AddListener(() =>
        { 
            _missionInfoText.text = "無し";
        });

        StoryManager.OnGetWindMission.AddListener(() =>
        {
            _missionInfoText.text = "大変！ふいごが使えない\n"+
            "風の力を呼び出そう！";
        });

        StoryManager.OnSlovedWindMission.AddListener(()=>
        {
            _missionInfoText.text = "無し";
        });
    }

    private void Update()
    {
        if (IsOpening())
        {
            _closeableDelayTimer += Time.deltaTime;
        }

        if (_closeableDelayTimer >= _closeableDelay)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if (IsOpening()) Close();
            }
        }
    }
    ////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////
    public void Open()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        // Focus on item slot
        var firstItem = _itemSlotParent.GetChild(0).gameObject;
        if (firstItem)
        {
            EventSystem.current.SetSelectedGameObject(firstItem);
            UpdateItemInfo();
        }
    }

    public void Close()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        _closeableDelayTimer = 0.0f;
    }

    public bool IsOpening()
    {
        return transform.GetChild(0).gameObject.activeSelf;
    }

    public bool AddItem(ItemAsset newItem)
    {
        if (_itemCount == _maxCapacity)
        {
            return false;
        }
        else
        {
            _itemSlots[_itemCount].
                GetComponent<ItemUI>().ItemInfo = newItem;
            ++_itemCount;
            return true;
        }
    }

    public void UpdateItemInfo()
    {
        var info = EventSystem.current.currentSelectedGameObject.GetComponent<ItemUI>().ItemInfo;
        if (info)
        {
            _itemInfoText.text = info.Desc;
        }
        else
        {
            _itemInfoText.text = "無し";
        }
    }
}