using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVariables
{
  public static GameObject highlight = null;
  public static GameObject selection = null;

  public static void MyStaticMethod()
  {
    Debug.Log("Static method called.");
  }
}
