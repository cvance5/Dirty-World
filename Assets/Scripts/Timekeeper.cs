using System;
using System.Collections;
using UnityEngine;

public class Timekeeper : Singleton<Timekeeper>
{
    public static Coroutine SetTimer(float delay, Action callback)
    {
        return Instance.StartCoroutine(WaitForTimer(delay, callback));
    }

    public static Coroutine SetTimer(int frames, Action callback)
    {
        return Instance.StartCoroutine(WaitForFrames(frames, callback));
    }

    private static IEnumerator WaitForTimer(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }

    private static IEnumerator WaitForFrames(int frames, Action callback)
    {
        while (frames > 0)
        {
            yield return new WaitForEndOfFrame();
            frames--;
        }

        callback?.Invoke();
    }
}