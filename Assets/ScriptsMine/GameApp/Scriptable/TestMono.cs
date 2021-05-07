
using System;
using Pangu.Const;
using  UnityEngine;

public class TestMono : MonoBehaviour, ISerializationCallbackReceiver
{
    //public TestScriptable asset;
    public GameObject refGo;

    public TestComponent refComp;
    
    public Transform dest;

    private void Awake()
    {
        
    }
    

    public void OnBeforeSerialize()
    {
        //Debug.Log($"OnBeforeSerialize: {GetInstanceID()}");
    }

    public void OnAfterDeserialize()
    {
        //Debug.Log($"OnAfterDeserialize: {GetInstanceID()}");
    }

    private void Update()
    {
        var t = refGo.transform;
        var curDir = Vector3.forward;
        var dir = dest.position - t.position;
        
        var rot1 = Quaternion.AngleAxis(Vector3.Angle(curDir, dir), Vector3.Cross(curDir, dir).normalized);
        
        var rot3 = Quaternion.Lerp(t.rotation, rot1, Time.deltaTime * 4);

        Debug.Log($"{rot3.eulerAngles}");
        
        t.rotation = rot3;
        //t.LookAt(dest);
    }

    private void OnDrawGizmos()
    {
        var preColor = Gizmos.color;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(refGo.transform.position, refGo.transform.forward * 10);
        Gizmos.color = preColor;
    }

    public static Quaternion Add(Quaternion rhs, Quaternion lhs)
    {
        var sign = Mathf.Sign(Quaternion.Dot(rhs, lhs));
        return new Quaternion(rhs.x + sign * lhs.x, rhs.y + sign * lhs.y, rhs.z + sign * lhs.z, rhs.w + sign * lhs.w);
    }
    
}