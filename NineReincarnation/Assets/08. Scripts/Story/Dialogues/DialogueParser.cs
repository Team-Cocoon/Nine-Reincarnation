using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string CSVFileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); // 대사 리스트 생성
        TextAsset csvData = Resources.Load<TextAsset>(CSVFileName);
        
        string[] data = csvData.text.Split(new char[] { '\n' });
        for (int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });

            Dialogue dialogue = new Dialogue();
            dialogue.eventID = row[0];
            dialogue.objectName = row[1];
            dialogue.eventName = row[2];
            dialogue.expression = row[3];
            dialogue.animName = row[4];
            dialogue.contexts = row[5];

            dialogueList.Add(dialogue);
        }
        return dialogueList.ToArray();
    }
}
