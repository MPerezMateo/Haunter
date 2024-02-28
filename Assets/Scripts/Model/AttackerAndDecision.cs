using System.Collections.Generic;

[System.Serializable]
public class AttackerAndDecision
{
  public Attacker attacker;
  public List<Decision> decisions;

  public static implicit operator bool(AttackerAndDecision obj)
  {
    // Define your logic to determine whether the object should be considered true or false
    return obj != null; // For example, consider the object true if it's not null
  }
}