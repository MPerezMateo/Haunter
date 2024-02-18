using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
  public Material defaultMaterial;
  public Material highLightMaterial;
  // Tiles may be breakable, therefore a counter for hits must be set.
  public void Start()
  {
    if (defaultMaterial == null)
    {
      Debug.LogError("Default Material is not assigned!");
      return;
    }

    // Set the default material on start
    SetMaterial(defaultMaterial);
  }

  public void SetMaterial(Material newMaterial)
  {
    // Apply the new material to the Renderer component
    GetComponent<Renderer>().material = newMaterial;
  }
}