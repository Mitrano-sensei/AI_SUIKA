using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static IEnumerator DelayedAction(float delayInSeconds, System.Action action)
    {
        yield return new WaitForSeconds(delayInSeconds);
        action();
    }

    public static void KillChildren(this Transform parent)
    {
        foreach (Transform child in parent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }   
}
