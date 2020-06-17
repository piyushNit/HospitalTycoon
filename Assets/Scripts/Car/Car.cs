using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Car
{
    [RequireComponent(typeof(Animator))]
    public class Car : CarBase
    {
        protected override void OnCarReachedDesignation()
        {
            base.OnCarReachedDesignation();
        }
    }
}
