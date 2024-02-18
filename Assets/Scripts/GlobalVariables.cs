using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVariables
{
  private static GameObject _highlight = null;
  public static GameObject Highlight
  {
    get { return _highlight; }
    set { /* Debug.Log($"New highlight item {value}"); */ _highlight = value; }
  }
  private static GameObject _selection = null;
  public static GameObject Selection
  {
    get { return _selection; }
    set { /* Debug.Log($"New selection item {value}"); */ _selection = value; }
  }
  public static void MyStaticMethod()
  {
    Debug.Log("Static method called.");
  }
}
