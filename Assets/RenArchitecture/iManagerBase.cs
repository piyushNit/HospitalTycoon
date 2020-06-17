
namespace Arch.Core
{
    public interface iManagerBase
    {
        /// <summary>
        /// This is a Initlizier and show initilize only once in the game
        /// This should get call at the beginning of the game
        /// </summary>
        /// <param name="_stateController"></param>
        void Initilize(StateController _stateController);

        void OnStateChange(StateController _stateController);

        /// <summary>
        /// Load data before game started
        /// </summary>
        void LoadGameData();

        /// <summary>
        /// Save game
        /// </summary>
        void SaveGameData();

        /// <summary>
        /// This is a native callbacks,
        /// Android : App Background / Foreground
        /// Desktop : Loose focus / in Focus
        /// </summary>
        /// <param name="focus"></param>
        void OnGameFocused(bool focus);
    }
}