using System;
using System.Collections;

namespace Utilities
{
    public class CoroutineHandler : Singleton<CoroutineHandler>
    {
        public static void RegisterCoroutine(Func<IEnumerator> coroutine) => Instance.StartCoroutine(coroutine());
    }
}