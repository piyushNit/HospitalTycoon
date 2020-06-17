using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Management
{
    public class GameplaySceneLoader : MonoBehaviour
    {
        [SerializeField] UnityEngine.UI.Slider m_sliderProgress;

        private void Start()
        {
            StartCoroutine(SceneLoad());
        }

        IEnumerator SceneLoad()
        {
            yield return null;

            //Begin to load the Scene you specify
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Gameplay_Final");
            //Don't let the Scene activate until you allow it to
            asyncOperation.allowSceneActivation = false;
            Debug.Log("Pro :" + asyncOperation.progress);
            //When the load is still in progress, output the Text and progress bar
            while (!asyncOperation.isDone)
            {
                //Output the current progress
                m_sliderProgress.value = asyncOperation.progress;

                // Check if the load has finished
                if (asyncOperation.progress >= 0.9f)
                {
                    //Change the Text to show the Scene is ready
                    //m_Text.text = "Press the space bar to continue";
                    //Wait to you press the space key to activate the Scene
                    //if (Input.GetKeyDown(KeyCode.Space))
                        //Activate the Scene
                        asyncOperation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}