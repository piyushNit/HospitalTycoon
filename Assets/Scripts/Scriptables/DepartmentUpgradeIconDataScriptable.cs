using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital.Scriptable
{
    [CreateAssetMenu(menuName = "Hospital/Department upgrades Icons")]
    public class DepartmentUpgradeIconDataScriptable : ScriptableObject
    {
        [SerializeField] Sprite[] icons;
        public Sprite[] Icons { get => icons; }
    }
}