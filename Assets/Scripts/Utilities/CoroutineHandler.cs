using System;
using System.Collections;
using System.Collections.Generic;

namespace Utilities
{
    public class CoroutineHandler : Singleton<CoroutineHandler>
    {
        private static readonly Dictionary<object, Queue<Func<IEnumerator>>> _queuedCoroutines
                          = new Dictionary<object, Queue<Func<IEnumerator>>>();

        public static void StartCoroutine(Func<IEnumerator> coroutine) => Instance.StartCoroutine(coroutine());
    }
}