using System;
using System.Collections.Generic;
using UnityEngine;

public enum ScriptStateType
{ 
    PRE,
    END,
    ING
}

public enum EventType
{ 
    AnumStory, //대사없이 애니메이션만 진행
    DialogueStory, //대사만 나오는 경우
    TextEventStory, //대사 중간중간에 애니메이션 변경 + 사운드 나올때
    Event //발생하는 이벤트 존재 (나무 클릭이나 카메라 흔들림 같은)
}

[Serializable]
public class MissionData
{
    public int StageID;
    public int LevelID;
    public ScriptStateType ScriptState;
    public int index;
    public string CharacterName; //캐릭터 이름
    public EventType Event; //이벤트 함수
    public string CharacterImageName; //캐릭터 이미지 이름
    public string CharacterAnimatino; //캐릭터 애니메이션
    public string Script;

}

[ExcelAsset (ExcelName = "DataTable", HeaderRow = 0, DataStartRow = 1, DataStartColumn = 0, AssetPath = "Resources", LogOnImport = true)]
public class DataTable : ScriptableObject
{
	public List<MissionData> Sheet1;
}
