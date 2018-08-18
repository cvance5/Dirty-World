using System;
using System.Collections;
using UnityEngine;

public class Timekeeper : Singleton<Timekeeper>
{
    public static Coroutine SetTimer(float delay, Action callback)
    {
        return Instance.StartCoroutine(WaitForTimer(delay, callback));
    }

    private static IEnumerator WaitForTimer(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}