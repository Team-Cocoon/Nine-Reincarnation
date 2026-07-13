using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCameraShiftHelper : MonoBehaviour
{
    [Serializable]
    struct CameraShiftObj
    {
        public string Name;
        public Transform objTransfrom;
    }

    [SerializeField] private List<CameraShiftObj> _objList = new List<CameraShiftObj>();
    private Dictionary<string, Transform> _objDict = new Dictionary<string, Transform> ();

    private void Awake()
    {
        foreach (CameraShiftObj obj in _objList) {
            _objDict[obj.Name] = obj.objTransfrom;
        }
    }

    public Transform GetShiftObjTransform(string name)
    {
        return _objDict[name];
    }
}
