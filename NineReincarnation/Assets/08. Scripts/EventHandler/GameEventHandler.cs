using System;

public static class GameEventHandler
{
    public static event Action ExitExcuted;

    #region 씬 상태 관련 이벤트
    public static event Action GameClearExcuted;

    public static event Action TitleExcuted;

    public static event Action StoryExcuted;

    public static event Action StageExcuted;
    #endregion

    #region 게임 플레이 상태 관련 이벤트
    public static event Action GameStartExcuted;
    #endregion


    #region Invoke 처리
    public static void ExitExcuted_Invoke() => ExitExcuted?.Invoke();
    public static void GameClearExcuted_Invoke() => GameClearExcuted?.Invoke();
    public static void TitleExcuted_Invoke() => TitleExcuted?.Invoke();
    public static void StoryExcuted_Invoke() => StoryExcuted?.Invoke();

    public static void StageExcuted_Invoke() => StageExcuted?.Invoke();
    public static void GameStartExcuted_Invoke() => GameStartExcuted?.Invoke();
    #endregion
}