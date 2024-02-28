using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : Moveable
{
  // Start is called before the first frame update
  public float baseDamage;
  public float speed;
  protected new void Start()
  {
    base.Start();
  }

  // Update is called once per frame
  void LateUpdate()
  {
    move();
  }

  void attack(GameObject go)
  {
    // Si el destino es una instancia de destructible o de atacker, vamos a reducir su HP
  }
}
