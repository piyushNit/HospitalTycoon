using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.UI
{
    public class UiUtilities : UiCoreHolder
    {
        [SerializeField] GameObject touchBlocker;
        [SerializeField] GameObject loadingIcon;
        [SerializeField] GameObject splashImage;
        public bool IsLoadingSpinnerActivated { get => loadingIcon.activeSelf; }

        /// <summary>
        /// Show/Hide loading spinner. Use when server calls are in progress
        /// And need to disable all UI touches
        /// </summary>
        /// <param name="enable"></param>
        public void LoadingSpinner(bool enable)
        {
            loadingIcon.SetActive(enable);
            touchBlocker.SetActive(enable);
        }

        public void ShowSplashScreen()
        {
            splashImage.SetActive(true);
            LoadingSpinner(true);
        }

        public void HideSplashScreen()
        {
            splashImage.SetActive(false);
            LoadingSpinner(false);
        }
    }
}