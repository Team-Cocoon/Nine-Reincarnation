using System;

public static class GameEventHandler
{
    #region 씬 상태 관련 이벤트
    public static Action GameClearExcuted;

    public static Action TitleExcuted;

    public static Action StoryExcuted;

    public static Action StageExcuted;

    public static Action ExitExcuted;
    #endregion

    #region 게임 플레이 상태 관련 이벤트
    public static Action GameStartExcuted;
    #endregion
}