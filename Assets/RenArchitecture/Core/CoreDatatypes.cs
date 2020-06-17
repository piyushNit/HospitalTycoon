namespace Arch.Core
{
    public enum State
    {
        PREGAME = 0,
        INGAME,
        POSTGAME
    }
    public enum SubStates
    {
        User_Login = 0,
        PregameLoad,
        PregameInit,
        PregameLevelSet,
        PregameUiUpdate,
        PregameFinished,
        IngameInit,
        IngameFinished,
        PostInit,
        Result,
        PostFinished,

        FTUE_IN_GAME_INIT,
        FTUE_PART_ONE_FINISHED
    }

}
