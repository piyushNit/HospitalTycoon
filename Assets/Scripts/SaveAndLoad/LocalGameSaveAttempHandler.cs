using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arch.Save
{
    public class LocalGameSaveAttempHandler
    {
        public void ProceedToSave(Management.SaveLoad.MasterLoader masterLoader)
        {
            Arch.Save.GameBinarySaveManager.SaveGame(masterLoader, Management.SaveLoad.KEYS.KEY_MASYER_LOADER,
                (bool success) =>
                {
                    CbOnSaveAttept(Management.SaveLoad.KEYS.KEY_MASYER_LOADER, success);
                });
        }

        void CbOnSaveAttept(string typeStr, bool success)
        {
            Debug.Log(string.Format("File Save attempt : {0}, {1}", typeStr, success));
        }

        /// <summary>
        /// Load game data from binary or server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyString"></param>
        /// <param name="loadFromBinary"></param>
        /// <returns></returns>
        public T LoadGame<T>(string keyString, bool loadFromBinary = false)
        {
            return Arch.Save.GameBinarySaveManager.LoadGame<T>(keyString);
        }
    }
}