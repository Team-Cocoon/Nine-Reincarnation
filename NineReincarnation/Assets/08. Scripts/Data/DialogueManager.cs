
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ExcelData;
using UnityEngine;

namespace DialogueSpace
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private int _id;
        [SerializeField] private DialogueUI _ui;
        [SerializeField] private StoryAnimationManager _storyAnimationManager;
        [SerializeField] private StoryEventManager _storyEventManager;

        private DialogueDB _dialogueDB = new();
        private int _nextId;

        private void Awake()
        {
            
        }

        public async UniTask NextDialogue()
        {
            DialogueClass dialogue = _dialogueDB.GetData<DialogueClass>(_id);
            
            _nextId = dialogue.ID;

            List<UniTask> tasks = new();
            if (dialogue.EventType != ExcelData.EventType.End)
            {
                if ((dialogue.EventType & ExcelData.EventType.Script) == ExcelData.EventType.Script)
                {
                    tasks.Add(_ui.UpdateUI(_dialogueDB.GetData<ScriptClass>(_id)));
                }
                if ((dialogue.EventType & ExcelData.EventType.Event) == ExcelData.EventType.Event)
                {
                    //tasks.Add(_storyEventManager);
                }
                if ((dialogue.EventType & ExcelData.EventType.Camera) == ExcelData.EventType.Camera)
                {

                }
                if ((dialogue.EventType & ExcelData.EventType.Animation) == ExcelData.EventType.Animation)
                {
                    tasks.Add(_storyAnimationManager.ExcuteAnimation(_dialogueDB.GetData<AnimationClass>(_id)));
                }
            }

            await UniTask.WhenAll(tasks);
            _id = _nextId;
        }
    }
}