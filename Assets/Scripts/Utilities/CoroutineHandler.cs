using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class CoroutineHandler : Singleton<CoroutineHandler>
    {
        private static readonly Dictionary<object, Queue<Func<IEnumerator>>> _queuedCoroutines
                          = new Dictionary<object, Queue<Func<IEnumerator>>>();

        public static Coroutine StartCoroutine(Func<IEnumerator> coroutine) => Instance.StartCoroutine(coroutine());
        public static void AbortCoroutine(Coroutine chunkActivationCoroutine) => Instance.StopCoroutine(chunkActivationCoroutine);
    }
}