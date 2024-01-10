using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public static IEnumerator DelayedAction(float delayInSeconds, System.Action action)
    {
        yield return new WaitForSeconds(delayInSeconds);
        action();
    }
}
