using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital.Scriptable
{
    [CreateAssetMenu(menuName = "Hospital/Department upgrades")]
    public class DepartmentUpgradeDataScriptable : ScriptableObject
    {
        [System.Serializable]
        public class JsonDataFileHolder
        {
            public string title;
            public Management.Hospital.Core.DepartmentType departmentType;
            public TextAsset textAssetFile;
            public DepartmentUpgradeIconDataScriptable iconData;

            [Range(0, 100)]
            [SerializeField] float increaseLevelUnlockPerBy = 20;
            public float IncreaseLevelUnlockPerBy { get => increaseLevelUnlockPerBy; }
        }

        [SerializeField] List<JsonDataFileHolder> jsonDataFileList;
        public List<JsonDataFileHolder> JsonDataFileList { get => jsonDataFileList; }

        public TextAsset GetTextAssets(Management.Hospital.Core.DepartmentType _type)
        {
            return JsonDataFileList.Find(obj => obj.departmentType == _type).textAssetFile;
        }

        /// <summary>
        /// Returns json data in string format
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        private string GetJsonString(Management.Hospital.Core.DepartmentType _type)
        {
            TextAsset textAssets = GetTextAssets(_type);
            if (textAssets == null)
            {
                Debug.LogError("DepartmentItem : SCRIPTABLE_ERROR => [" + _type.ToString() + "] json item not specified");
                //Management.Errors.GameErrorHandler.LogError<Management.Hospital.Scriptable.DepartmentUpgradeDataScriptable>(this, "DepartmentItem : SCRIPTABLE_ERROR => [" + _type.ToString() + "] json item not specified");
                return "";
            }
            return textAssets.text;
        }

        /// <summary>
        /// retuns json data in <T> model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_type"></param>
        /// <returns></returns>
        public ClassName GetJsonModel<ClassName>(Management.Hospital.Core.DepartmentType _type)
        {
            string jsonString = GetJsonString(_type);
            return Arch.Json.JsonReader.LoadJson<ClassName>(jsonString);
        }

        /// <summary>
        /// Get Upgrade icon
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Sprite GetIconFromIndex(Management.Hospital.Core.DepartmentType _type, int index)
        {
            return GetIconScriptable(_type).Icons[index];
        }

        /// <summary>
        /// Get icon scriptable data
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public DepartmentUpgradeIconDataScriptable GetIconScriptable(Management.Hospital.Core.DepartmentType _type)
        {
            return JsonDataFileList.Find(obj => obj.departmentType == _type).iconData;
        }

        /// <summary>
        /// Get level increase percentage
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public float GetLevelIncreasePercentage(Management.Hospital.Core.DepartmentType _type)
        {
            return JsonDataFileList.Find(obj => obj.departmentType == _type).IncreaseLevelUnlockPerBy;
        }
    }
}