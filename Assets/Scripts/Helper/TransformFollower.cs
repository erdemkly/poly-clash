using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct MyAxis
{
    public bool x, y, z;
}
[Serializable]
public struct MyClamp
{
    public bool x, y, z;
    [EnableIf("x")]public float minX, maxX;
    [EnableIf("y")]public float minY, maxY;
    [EnableIf("z")]public float minZ, maxZ;
}
public class TransformFollower : MonoBehaviour
{
    [SerializeField,Required()] public Transform target;

    public MyAxis positionAxis,scaleAxis,eulerAxis,quaternionAxis;
    public MyClamp clampedAxis;
    [SerializeField] public Vector3 posOffset;

    private Camera _cam;
    public enum FollowType
    {
        WorldToWorld,
        ScreenToWorld,
        WorldToScreen,
        ScreenToScreen
    }
    public FollowType followType;

    public float smoothness = 0.125f;

    private void OnEnable()
    {
        if (_cam == null) _cam = Camera.main;
    }

    [Button("Initialize")]
    private void Start()
    {
        Follow(1);
    }
    private void LateUpdate()
    {
        Follow(smoothness);
    }
    private Vector3 InitFollowType()
    {
        Vector3 targetPos;
        switch (followType)
        {
            case FollowType.WorldToWorld:
                targetPos = target.position + posOffset;
                break;
            case FollowType.ScreenToWorld:
                targetPos = _cam.WorldToScreenPoint(target.position+posOffset);
                targetPos.z = 0;
                break;
            case FollowType.WorldToScreen:
                targetPos = _cam.ScreenToWorldPoint(target.position+posOffset);
                break;
            case FollowType.ScreenToScreen:
                targetPos = target.position+posOffset;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return targetPos;
    }
    private void SetPosition(float s)
    {
        if (!positionAxis.x && !positionAxis.y && !positionAxis.z) return;
        var pos = transform.position;
        var targetPos = InitFollowType();
        if (positionAxis.x)
        {
            if (clampedAxis.x) targetPos.x = Mathf.Clamp(targetPos.x, clampedAxis.minX, clampedAxis.maxX);
            pos.x = targetPos.x;
        }
        if (positionAxis.y)
        {
            if (clampedAxis.y) targetPos.y = Mathf.Clamp(targetPos.y, clampedAxis.minY, clampedAxis.maxY);
            pos.y = targetPos.y;
        }
        if (positionAxis.z)
        {
            if (clampedAxis.z) targetPos.z = Mathf.Clamp(targetPos.z, clampedAxis.minZ, clampedAxis.maxZ);
            pos.z = targetPos.z;
        }
        transform.position = Vector3.Lerp(transform.position, pos, s);
    }
    private void SetScale(float s)
    {
        if (!scaleAxis.x && !scaleAxis.y && !scaleAxis.z) return;
        
        var localScale = transform.localScale;
        
        if (scaleAxis.x) localScale.x = target.localScale.x;
        if (scaleAxis.y) localScale.y = target.localScale.y;
        if (scaleAxis.z) localScale.z = target.localScale.z;
        
        transform.localScale = Vector3.Lerp(transform.localScale, localScale, s);
    }
    private void SetEuler()
    {
        if (!eulerAxis.x && !eulerAxis.y && !eulerAxis.z) return;
        
        var euler = transform.eulerAngles;
        
        if (eulerAxis.x) euler.x = target.eulerAngles.x;
        if (eulerAxis.y) euler.y = target.eulerAngles.y;
        if (eulerAxis.z) euler.z = target.eulerAngles.z;

        transform.eulerAngles = euler;
    }
    private void SetQuaternion(float s)
    {
        if (!quaternionAxis.x && !quaternionAxis.y && !quaternionAxis.z) return;
        
        var quaternion = transform.rotation;
        
        if (quaternionAxis.x) quaternion.x = target.rotation.x;
        if (quaternionAxis.y) quaternion.y = target.rotation.y;
        if (quaternionAxis.z) quaternion.z = target.rotation.z;

        transform.rotation = Quaternion.Lerp(transform.rotation,quaternion,s);
    }
    private void Follow(float s)
    {
        if (target == null) return;
        SetPosition(s);
        SetScale(s);
        SetEuler();
        SetQuaternion(s);
    }


}
