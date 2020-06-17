using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Management.SimpleTween
{
    public class SelfHoverAnimation : MonoBehaviour
    {
        [SerializeField] float hoverHeight = 5;
        [SerializeField] float hoverDuration = 1;
        [SerializeField] bool repeat = true;

        private void OnEnable()
        {
            transform.DOMove(new Vector3(transform.position.x, transform.position.y + hoverHeight, transform.position.z),
                hoverDuration).SetLoops(repeat ? -1 : 1, LoopType.Yoyo);
        }

        private void OnDisable()
        {
            transform.DOKill();
        }
    }
}