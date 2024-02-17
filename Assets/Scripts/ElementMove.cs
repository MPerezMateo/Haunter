using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Element : Moveable
{
  // Start is called before the first frame update
  protected new void Start()
  {
    base.Start();
  }

  // Update is called once per frame
  void Update()
  {
    move();//
  }
}
