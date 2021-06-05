using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        Vector3 TargetDir = Vector3.ProjectOnPlane(this.transform.position,
            Camera.main.transform.forward);
        this.transform.rotation = Quaternion.LookRotation(TargetDir);
    }
}