using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selection : MonoBehaviour
{
  public Material highlightMaterial;
  public Material selectionMaterial;

  private Material originalMaterialHighlight;
  private Material originalMaterialSelection;
  private RaycastHit raycastHit;
  private List<string> selectableTags = new List<string>() { "Selectable", "Pathable" };
  void Update()
  {
    // Highlight
    if (GlobalVariables.highlight != null)
    {
      GlobalVariables.highlight.GetComponent<MeshRenderer>().sharedMaterial = originalMaterialHighlight;
      GlobalVariables.highlight = null;
    }
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit)) //Make sure you have EventSystem in the hierarchy before using EventSystem
    {
      GlobalVariables.highlight = raycastHit.collider.gameObject;

      if (selectableTags.Contains(GlobalVariables.highlight.tag) && GlobalVariables.highlight != GlobalVariables.selection)
      {
        if (GlobalVariables.highlight.GetComponent<MeshRenderer>().material != highlightMaterial)
        {
          originalMaterialHighlight = GlobalVariables.highlight.GetComponent<MeshRenderer>().material;
          GlobalVariables.highlight.GetComponent<MeshRenderer>().material = highlightMaterial;
        }
      }
      else
      {
        GlobalVariables.highlight = null;
      }
    }

    // Selection
    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
    {
      if (GlobalVariables.highlight)
      {
        if (GlobalVariables.selection != null)
        {
          GlobalVariables.selection.GetComponent<MeshRenderer>().material = originalMaterialSelection;
        }
        GlobalVariables.selection = raycastHit.collider.gameObject;
        if (GlobalVariables.selection.GetComponent<MeshRenderer>().material != selectionMaterial)
        {
          originalMaterialSelection = originalMaterialHighlight;
          GlobalVariables.selection.GetComponent<MeshRenderer>().material = selectionMaterial;
        }
        GlobalVariables.highlight = null;
      }
      else
      {
        if (GlobalVariables.selection)
        {
          GlobalVariables.selection.GetComponent<MeshRenderer>().material = originalMaterialSelection;
          GlobalVariables.selection = null;
        }
      }
    }

  }

}