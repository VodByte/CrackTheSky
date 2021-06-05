using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GHSUtility
{
    public static void CheckNull(object o)
    {
        if (o == null)
        {
            Debug.LogError($"{o} is null");
            Debug.Break();
        }
    }

    public static Vector2 WorldToViewportPoint(Vector3 worldPos)
    {
        /// Trans world coord to UI coord.
        /// Viewport coord: 0,0 ~ 1,1. Origin is bottom left.
        /// canvas coord: 0,0 ~ screenSize. Origin is bottom left.
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(worldPos);
        float canvasPosX = viewportPos.x * Screen.width;
        float canvasPosY = viewportPos.y * Screen.height;
        return new Vector2(canvasPosX, canvasPosY);
    }

    public static UnityEngine.Object LoadPrefabFromFile(string filename)
    {
        var loadedObject = Resources.Load("Prefabs/" + filename);
        if (loadedObject == null)
        {
            throw new UnityException("...no file found - please check the configuration");
        }
        return loadedObject;
    }

    public static bool Approximately(this Quaternion quatA, Quaternion value, float acceptableRange)
    {
        return 1 - Mathf.Abs(Quaternion.Dot(quatA, value)) < acceptableRange;
    }

    public static Texture2D TextureFromSprite(Sprite sprite)
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

    private static bool _isPressedCrossKey = false;
    public static bool GetCrossKeyDown()
    {
        bool isPressing = Input.GetAxis("Horizontal_Inventory") != 0 || 
            Input.GetAxis("Vertical_Inventory") != 0;

        if (isPressing)
        {
            if (_isPressedCrossKey == false)
            {
                _isPressedCrossKey = true;
                return true;
            }
        }
        else
        {
            _isPressedCrossKey = false;
        }

        return false;
    }

    public static bool GetAnyGamepadKey()
    {
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown("joystick 1 button " + i))
            {
                return true;
            }
        }

        return false;
    }
}