using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using utils;
public class Destructible : MonoBehaviour
{
  [SerializeField] private Material goodState;
  [SerializeField] private Material damagedState;
  [SerializeField] private Image HpUI;
  [SerializeField] private TMP_Text HpText;
  private int _health;
  public int Health
  {
    get { return _health; }
    set
    {
      _health = Math.Clamp(value, 0, maxHealth);
      // Any time it is changed, we refresh the UI. We shoudl do this in a linear way with a splash/ animation and sound
      UIRefreshHealth(_health);
    }
  }

  private void UIRefreshHealth(int health)
  {
    if (HpUI)
    {
      Vector2 delta = HpUI.rectTransform.sizeDelta;
      delta.y = (float)health / maxHealth * 0.8f;
      HpUI.rectTransform.sizeDelta = delta;
      HpText.text = $"{health} / {maxHealth}";
    }
  }

  public int maxHealth { get; set; }

  // Deberíamos otorgar a los destruibles armadura, en funcion del tipo de daño que puedan recibir:
  // Ej sacos son debiles a estocadas, cristales son débiles a golpes, cuerdas débiles a cortes

  // Start is called before the first frame update
  void Start()
  {
    maxHealth = 120;
    Health = 120;
    StartCoroutine(timers.Sleep(5f, () => { Health = 40; }));
  }

  // Update is called once per frame
  void Update()
  {

  }

  void takeDamage()
  {

  }

}
