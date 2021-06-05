using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField]
    private ItemAsset _ItemInfo = null;

    private void Awake()
    {
        if (_ItemInfo)
        {
            GetComponent<Image>().sprite = ItemInfo.Icon;
        }
    }

    public ItemAsset ItemInfo
    {
        get 
        {
            return _ItemInfo; 
        }
        set
        {
            gameObject.GetComponent<Image>().sprite = value.Icon;
            _ItemInfo = value;
        }
    }

    public void OnChangeSelect()
    {
        InventoryManager.Instance.UpdateItemInfo();
    }
}