using System.Collections;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Runtime.Serialization.Formatters.Binary;

namespace Management.Services
{
    public class GooglePlayServices : MonoBehaviour
    {
        private static GooglePlayServices instance;
        public static GooglePlayServices Instance { get => instance; }

        GooglePlayGames.BasicApi.SavedGame.ISavedGameMetadata cloudCurrentGame = null;

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            // enables saving game progress.
            .EnableSavedGames()
            // requests a server auth code be generated so it can be passed to an
            //  associated back end server application and exchanged for an OAuth token.
                //.RequestServerAuthCode(false)
            // requests an ID token be generated.  This OAuth token can be used to
            //  identify the player to other services such as Firebase.
                //.RequestIdToken()
            .Build();

            // recommended for debugging:
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.InitializeInstance(config);
            // Activate the Google Play Games platform
            PlayGamesPlatform.Activate();


            PlayGamesPlatform.Instance.Authenticate((result)=> {
                #if TEST_GAME
                Debug.Log("Sign in Ststus: " + result.ToString());
                #endif
            }, true);
        }

        #region GOOGLE_SIGN_IN

        /// <summary>
        /// Checks if the user is already authenticated
        /// </summary>
        /// <returns></returns>
        public bool IsUserAuthenticated()
        {
            #if UNITY_EDITOR
            return false;
            #endif
            return Social.localUser.authenticated;
        }


        public void SignIn(System.Action<bool> signInCallback)
        {
            #if UNITY_EDITOR
            if(signInCallback != null)
                signInCallback(false);
            return;
            #endif
            PlayGamesPlatform.Instance.Authenticate(signInCallback, false);
        }

        public void SignOut()
        {
            #if UNITY_EDITOR
            return;
            #endif
            PlayGamesPlatform.Instance.SignOut();
        }
        #endregion

        #region GOOGLE_SAVE_GAME
        /// <summary>
        /// Read the save file in google cloud
        /// </summary>
        /// <param name="cbOnReadScore"></param>
        public void ReadFromGoogleCloud(System.Action<SaveLoad.MasterLoader> cbOnReadScore)
        {
            // CALLBACK: Handle the result of a binary read
            System.Action<SavedGameRequestStatus, byte[]> readCallback = (SavedGameRequestStatus status, byte[] data) =>
            {
                #if TEST_GAME
                Debug.Log("Read from google cloud: " + status.ToString());
                #endif
                if (status == SavedGameRequestStatus.Success && data != null)
                {
                    try
                    {
                        SaveLoad.MasterLoader masterLoader = (SaveLoad.MasterLoader)ByteArrayToObject(data);
                        cbOnReadScore(masterLoader);
                    }
                    catch
                    {
                        #if TEST_GAME
                        Debug.Log("GAMEERRE: Google Read Failed");
                        #endif
                        AnalyticsManager.Instance.CustomEvent("GAMEERRE: Google Read Failed");
                    }
                }
                else
                {
                    cbOnReadScore(null);
                }
            };

            ReadSavedGame(SaveLoad.KEYS.KEY_MASYER_LOADER, (SavedGameRequestStatus savedGameStatus, ISavedGameMetadata savedGameMeta) =>
            {
                cloudCurrentGame = savedGameMeta;
                ReadBinaryFile(savedGameMeta, readCallback);
            });
        }

        private void ReadSavedGame(string filename,
                             System.Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
        {

            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(
                filename,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                callback);
        }

        private byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }


        // Convert a byte array to an Object
        private object ByteArrayToObject(byte[] arrBytes)
        {
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, System.IO.SeekOrigin.Begin);
            object obj = (object)binForm.Deserialize(memStream);
            return obj;
        }

        /// <summary>
        /// Saves the game onto google cloud
        /// </summary>
        /// <param name="masterLoaderSave"></param>
        /// <param name="cbOnWriteCallback"></param>
        public void WriteScore(SaveLoad.MasterLoader masterLoaderSave, System.Action<SavedGameRequestStatus> cbOnWriteCallback)
        {
            // Local variable
            ISavedGameMetadata currentGame = null;

            // CALLBACK: Handle the result of a write
            System.Action<SavedGameRequestStatus, ISavedGameMetadata> writeCallback =
            (SavedGameRequestStatus status, ISavedGameMetadata game) => {
                #if TEST_GAME
                Debug.Log("(Lollygagger) Saved Game Write: " + status.ToString());
                #endif
            };

            // CALLBACK: Handle the result of a binary read
            System.Action<SavedGameRequestStatus, byte[]> readBinaryCallback =
            (SavedGameRequestStatus status, byte[] data) => {
                #if TEST_GAME
                Debug.Log("(HOSPIYAL_TYCOON) Saved Game Binary Read: " + status.ToString());
                #endif
                if (status == SavedGameRequestStatus.Success)
                {
                    try
                    {
                        byte[] newData = ObjectToByteArray(masterLoaderSave);
                        // Write new data
                        WriteSavedGame(currentGame, newData, writeCallback);
                    }
                    catch
                    {
                        #if TEST_GAME
                        Debug.Log("GAME_ERROR : Google Save Failed");
                        #endif
                        AnalyticsManager.Instance.CustomEvent("GAME_ERROR : Google Save Failed");
                    }
                }
            };

            // CALLBACK: Handle the result of a read, which should return metadata
            System.Action<SavedGameRequestStatus, ISavedGameMetadata> readCallback =
            (SavedGameRequestStatus status, ISavedGameMetadata game) => {
                #if TEST_GAME
                Debug.Log("(HOSPIYAL_TYCOON) Saved Game Read: " + status.ToString());
                #endif
                if (status == SavedGameRequestStatus.Success)
                {
                    // Read the binary game data
                    currentGame = game;
                    PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game,
                                                        readBinaryCallback);
                }
            };

            // Read the current data and kick off the callback chain
            #if TEST_GAME
            Debug.Log("(HOSPIYAL_TYCOON) Saved Game: Reading");
            #endif
            ReadSavedGame(SaveLoad.KEYS.KEY_MASYER_LOADER, readCallback);
        }

        private void WriteSavedGame(ISavedGameMetadata game, byte[] savedData,
                               System.Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
        {

            SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder()
                .WithUpdatedPlayedTime(System.TimeSpan.FromMinutes(game.TotalTimePlayed.Minutes + 1))
                .WithUpdatedDescription("Saved at: " + System.DateTime.Now);

            // You can add an image to saved game data (such as as screenshot)
            // byte[] pngData = <PNG AS BYTES>;
            // builder = builder.WithUpdatedPngCoverImage(pngData);

            SavedGameMetadataUpdate updatedMetadata = builder.Build();

            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.CommitUpdate(game, updatedMetadata, savedData, callback);
        }

        private void ReadBinaryFile(ISavedGameMetadata game, System.Action<SavedGameRequestStatus, byte[]> readCallback)
        {
            PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, readCallback);
        }

        /// <summary>
        /// Deletes the file in google cloud
        /// </summary>
        public void DeleteSaveFile()
        {
            ReadFromGoogleCloud((SaveLoad.MasterLoader masterLoader) => 
            {
                if (masterLoader != null && cloudCurrentGame != null)
                {
                    PlayGamesPlatform.Instance.SavedGame.Delete(cloudCurrentGame);
                    cloudCurrentGame = null;
                }
            });
        }
        #endregion
    }
}