using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiVfxSparkle : MonoBehaviour
{
    Coroutine coroutine = null;
    public void UpdateSparkle()
    {
        gameObject.SetActive(true);
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(DisableSparkle());
    }

    IEnumerator DisableSparkle()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }
}
