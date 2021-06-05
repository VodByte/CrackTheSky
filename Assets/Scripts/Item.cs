using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemAsset _itemInfo = null;
    public ItemAsset ItemInfo
    {
        get
        {
            Destroy(gameObject, 0.1f);
            return _itemInfo;
        }
        private set { }
    }

    private void Awake()
    {
        if (_itemInfo == null)
        {
            Debug.Log("Not assign Item info");
            return;
        }

        ItemInfo = _itemInfo;
        var mat = this.GetComponent<Renderer>().material;
        if (_itemInfo && mat)
        {
            mat.SetTexture("_MainTex", Sprite2Texture2D(_itemInfo.Icon));
        }
    }

////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////
    private Texture2D Sprite2Texture2D(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }
}
