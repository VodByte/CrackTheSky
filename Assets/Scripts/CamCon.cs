using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;

public class CamCon : MonoBehaviour
{
    [SerializeField]
    private float _distToRoot = 20f;
    [SerializeField]
    private float _rotSpeed = 20f;
    [SerializeField]
    private Transform _root = null;
    [SerializeField]
    private float _initiPolar = -90f;
    [SerializeField]
    private float _initiElevation = 30f;

    private float currentPolar;
    private float currentElevation;

    private void UpdatePos()
    {
        if (_root == null) return;

        this.transform.position =
            _root.transform.position +
            SphericalToCartesian(_distToRoot, currentPolar, currentElevation);
    }

    private void Awake()
    {
        currentElevation = _initiElevation;
        currentPolar = _initiPolar;
    }

    private void Start()
    {
        UpdatePos();
    }

    void Update()
    {
        var mouseDelta = Vector2.one * Input.GetAxis("MouseX") * Time.deltaTime * _rotSpeed;
        //var mouseDelta = InputSystem.GetDevice<Mouse>().
        //    delta.ReadValue() * Time.deltaTime * _rotSpeed;
        currentPolar += mouseDelta.x;
        currentElevation += -mouseDelta.y;
        UpdatePos();
        
        this.transform.rotation = Quaternion.LookRotation(
            _root.transform.position - this.transform.position);

    }

    public Vector3 SphericalToCartesian(float radius, float polar, float elevation)
    {
        polar = Mathf.Deg2Rad * polar;
        elevation = Mathf.Deg2Rad* elevation;
        float a = radius * Mathf.Cos(elevation);
        return new Vector3(a * Mathf.Cos(polar),
            radius * Mathf.Sin(elevation),
            a * Mathf.Sin(polar));
    }
}