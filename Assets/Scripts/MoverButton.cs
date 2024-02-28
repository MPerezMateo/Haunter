using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverButton : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
  }

  // Update is called once per frame
  void Update()
  {
  }

  public void makeMove()
  {
    // Aquí tenemos que hacer una función que lea el selectable y ejecute la función
    if (GlobalVariables.Selection.GetComponent<Moveable>().highlighted)
    {
      GlobalVariables.Selection.GetComponent<Moveable>().unSelectToMove();
    }
    else if (GlobalVariables.Selection.GetComponent<Moveable>() != null && !GlobalVariables.CurrentAttacker.decisions.Contains(Decision.Move))
    {
      GlobalVariables.Selection.GetComponent<Moveable>().selectToMove();
      //GlobalVariables.CurrentAttacker.decisions.Add(Decision.Move);
    }

  }
}
