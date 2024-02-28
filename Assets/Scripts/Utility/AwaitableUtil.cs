using System;
using System.Threading.Tasks;
using UnityEngine;

public static class AwaitableUtil
{

    //public static Vector3 MakeVector3(this byte value)
    //{
    //    return new Vector3(value, value, value);
    //}

    public static async Task WaitUntil(this Func<bool> predicate, int sleep = 1000, int timeOut = 20)
    {
        float attempts = 0;

        while (!predicate())
        {
            if (attempts >= 20)
            {
                Debug.Log($"Request timed out after {timeOut} attempts");
                break;
            }
            await Awaitable.WaitForSecondsAsync(sleep);
        }
    }
}
