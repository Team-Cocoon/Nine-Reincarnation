
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ExcelData;
using UnityEngine;
using VContainer;

namespace DialogueSpace
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private int _id;
        [Inject] private DialogueUI _ui;
        [Inject] private StoryAnimationManager _storyAnimationManager;
        [Inject] private StoryEventManager _storyEventManager;
        [Inject] private DialogueDB _dialogueDB;

        private int _nextId;

        public async UniTask NextDialogue()
        {
            DialogueClass dialogue = _dialogueDB.GetData<DialogueClass>(_id);

            _nextId = dialogue.ID;

            List<UniTask> tasks = new();

            //End면 종료
            if (dialogue.EventType != ExcelData.EventType.End)
            {
                if ((dialogue.EventType & ExcelData.EventType.Script) == ExcelData.EventType.Script)
                {
                    tasks.Add(_ui.UpdateUI(_dialogueDB.GetData<ScriptClass>(_id)));
                }
                if ((dialogue.EventType & ExcelData.EventType.Event) == ExcelData.EventType.Event)
                {
                    tasks.Add(_storyEventManager.ExcuteEvent());
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
            await UniTask.WaitForSeconds(dialogue.Duration);
            _id = _nextId;
        }
    }
}