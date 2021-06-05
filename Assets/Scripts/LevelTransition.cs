using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    public static LevelTransition Instance = null;

    [SerializeField]
    private float _rotSpeed = 110f;

    private IEnumerator _curCoroutine = null;
    private RectTransform _rectTrans = null;

    private void Awake()
    {
        if (Instance != null) Destroy(Instance);
        Instance = this;
        _rectTrans = transform.GetComponentInChildren<RectTransform>();
    }

    public void WipeScreen(System.Action action = null)
    {
        if (_curCoroutine == null)
        {
            _curCoroutine = RotBlackMask(action);
            StartCoroutine(_curCoroutine);
        }
        else
        {
            Debug.Log("Coroutine is running");
        }
    }

    private IEnumerator RotBlackMask(System.Action action)
    {
        Quaternion tarRot = _rectTrans.rotation * Quaternion.Euler(0, 0, 180f);
        while (!_rectTrans.rotation.Approximately(tarRot, 0.0001f))
        {
            _rectTrans.rotation *= Quaternion.Euler(0, 0, -_rotSpeed * Time.deltaTime);
            yield return null;
        }
        _rectTrans.rotation = tarRot;
        _curCoroutine = null;
        action?.Invoke();
    }
}