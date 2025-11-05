using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExcelData
{
    [Flags]
    public enum EventType
    {
        End       = 0b000000000,
        Animation = 0b000000001,
        Camera    = 0b000000010,
        Event     = 0b000000100,
        Wait      = 0b000001000,
        Script    = 0b000010000,
        Bubble    = 0b000100000
    }

    public enum CameraEventType
    {
        ZoomIn,
        ZoomOut,
        Shake
    }
    [Serializable]
    public class DialogueClass
    {
        public int ID;
        public EventType EventType; //이벤트 함수
        public int NextID;
    }

    [Serializable]
    public class ScriptClass
    {
        public int ID;
        public string Name;
        public string ImageName;
        public string Script;
    }


    [Serializable]
    public class AnimationClass
    {
        public int ID;
        public string Name;
        public string AnimationName;
        public float Duration;
    }

    [Serializable]
    public class CameraClass
    {
        public int ID;
        public CameraEventType Type;
        public float Size;
        public float Duration;
    }

    [Serializable]
    public class BubbleClass
    {
        public int ID;
        public string Name;
        public string Script;
    }


    [ExcelAsset(ExcelName = "DialogueData", HeaderRow = 0, DataStartRow = 1, DataStartColumn = 0, AssetPath = "Resources", LogOnImport = true)]
    public class DialogueData : ScriptableObject
    {
        //변수명은 시트 명으로
        public List<DialogueClass> Dialogue;
        public List<AnimationClass> Animation;
        public List<ScriptClass> Script;
        public List<CameraClass> Camera;
        public List<BubbleClass> Bubble;
    }
}