using ExcelData;
using UnityEngine;

namespace DialogueSpace
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private int _id;
        [SerializeField] private DialogueUI _ui;
        [SerializeField] private StoryAnimationManager _saManager;
        //[SerializeField] private StoryCameraManager _ui;

        private DialogueDB _dialogueDB = new();
        private int _nextId;

        private void Awake()
        {
            
        }

        public void NextDialogue()
        {
            DialogueClass dialogue = _dialogueDB.GetData<DialogueClass>(_id);

            _nextId = dialogue.ID;
            if (dialogue.EventType != ExcelData.EventType.End)
            {
                if ((dialogue.EventType & ExcelData.EventType.Script) == ExcelData.EventType.Script)
                {
                    _ui.UpdateUI(_dialogueDB.GetData<ScriptClass>(_id));
                }
                if ((dialogue.EventType & ExcelData.EventType.Event) == ExcelData.EventType.Event)
                {

                }
                if ((dialogue.EventType & ExcelData.EventType.Camera) == ExcelData.EventType.Camera)
                {

                }
                if ((dialogue.EventType & ExcelData.EventType.Animation) == ExcelData.EventType.Animation)
                {
                    _saManager.ExcuteAnimation(_dialogueDB.GetData<AnimationClass>(_id));
                }
            }
            _id = _nextId;
        }
    }
}