using System;
using System.Collections.Generic;
using ExcelData;
using UnityEngine;

public class DialogueDB
{
    private Dictionary<int, DialogueClass> _dialogues;
    private Dictionary<int, CameraClass> _cameraEvents;
    private Dictionary<int, AnimationClass> _animations;

    public DialogueDB()
    {
        _dialogues = new Dictionary<int, DialogueClass>();
        _cameraEvents = new Dictionary<int, CameraClass>();
        _animations = new Dictionary<int, AnimationClass>();
        DialogueData res = Resources.Load<DialogueData>("DialogueData");

        int[] ad = new int[5];

        foreach(DialogueClass dialogue in res.Dialogue)
        {
            int key = dialogue.ID;

            _dialogues.Add(key, dialogue);
        }

        foreach (CameraClass camera in res.Camera)
        {
            int key = camera.ID;

            _cameraEvents.Add(key, camera);
        }

        foreach (AnimationClass animation in res.Animation)
        {
            int key = animation.ID;

            _animations.Add(key, animation);
        }
    }

    public DialogueClass GetDialogue(int id)
    {
        if(_dialogues.ContainsKey(id))
        {
            return _dialogues[id];
        }

        return null;
    }

    public CameraClass GetCameraEvent(int id)
    {
        if (_cameraEvents.ContainsKey(id))
        {
            return _cameraEvents[id];
        }

        return null;
    }

    public AnimationClass GetAnimationEvent(int id)
    {
        if (_animations.ContainsKey(id))
        {
            return _animations[id];
        }

        return null;
    }
}
