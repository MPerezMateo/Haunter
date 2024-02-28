using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace utils
{
  public static class timers
  {
    public delegate void CallbackFunction();

    public static IEnumerator Sleep(float delayInSeconds, CallbackFunction callback)
    {
      // Wait for the specified time
      yield return new WaitForSeconds(delayInSeconds);

      // This code will be executed after the specified delay
      if (callback != null)
      {
        callback.Invoke();
      }
    }
  }
}