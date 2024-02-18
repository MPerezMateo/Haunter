using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVariables
{
  // Esta variable define si podemos seleccionar elementos en la ui o no.
  // Durante animaciones de movimiento y o ataques y turnos del enemigo, no podremos seleccionarlos.
  public static bool selectable = true;
  // Esta variable lee sobre qué objeto estamos haciendo hover
  private static GameObject _highlight = null;
  public static GameObject Highlight
  {
    get { return _highlight; }
    set { /* Debug.Log($"New highlight item {value}"); */ _highlight = value; }
  }
  // Esta variable lee qué objeto hemos seleccionado
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
