using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Arch.Save
{
    public static class GameBinarySaveManager
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("HospitalTycoon/Delete Save Game")]
        public static void DeleteSaveData()
        {
            DeleteFile(Management.SaveLoad.KEYS.KEY_MASYER_LOADER);
            PlayerPrefs.DeleteAll();
            Debug.Log("All save data deleted");
        }

        public static void DeleteFile(string keyString)
        {
            string filePath = GenerateFilePath(keyString);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        #endif
        /// <summary>
        /// Dont forget to give appropriate KeyString otherwise data will override and wont get back
        /// </summary>
        /// <param name="gameData"></param>
        /// <param name="keyString"></param>
        public static void SaveGame(object gameData, string keyString, System.Action<bool> callbackFileSaved = null)
        {
            if (gameData == null)
                return;
            BinaryFormatter binary = new BinaryFormatter();
            string filePath = GenerateFilePath(keyString);
            try
            {
                using (FileStream file = File.Create(filePath))
                {
                    binary.Serialize(file, gameData);
                    file.Close();
                }
                if (callbackFileSaved != null)
                    callbackFileSaved(true);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Game Error! " + ex.ToString());
                if (callbackFileSaved != null)
                    callbackFileSaved(false);
            }
            //FileStream file = File.Create(filePath);
            
        }

        public static T LoadGame<T>(string keyString)
        {
            string filePath = GenerateFilePath(keyString);
            if (File.Exists(filePath))
            {
                BinaryFormatter binary = new BinaryFormatter();
                FileStream file = File.Open(filePath, FileMode.Open);
                T tempObj = (T)binary.Deserialize(file);
                file.Close();
                return tempObj;
            }

            return default(T);
        }

        static string GenerateFilePath(string keyString)
        {
            return Application.persistentDataPath + "/" + keyString + ".save";
        }
    }
}