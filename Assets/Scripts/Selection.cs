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
  private List<string> selectableTags = new List<string>() { "Selectable", "Pathable", "Player" };
  void Update()
  {
    // Highlight
    if (GlobalVariables.selectable)
    {
      if (GlobalVariables.Highlight != null)
      {
        GlobalVariables.Highlight.GetComponent<MeshRenderer>().sharedMaterial = originalMaterialHighlight;
        GlobalVariables.Highlight = null;
      }
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit)) //Make sure you have EventSystem in the hierarchy before using EventSystem
      {
        GlobalVariables.Highlight = raycastHit.collider.gameObject;

        if (selectableTags.Contains(GlobalVariables.Highlight.tag) && GlobalVariables.Highlight != GlobalVariables.Selection)
        {
          if (GlobalVariables.Highlight.GetComponent<MeshRenderer>().material != highlightMaterial)
          {
            originalMaterialHighlight = GlobalVariables.Highlight.GetComponent<MeshRenderer>().material;
            GlobalVariables.Highlight.GetComponent<MeshRenderer>().material = highlightMaterial;
          }
        }
        else
        {
          GlobalVariables.Highlight = null;
        }
      }

      // Selection
      if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
      {
        if (GlobalVariables.Highlight)
        {
          if (GlobalVariables.Selection != null)
          {
            GlobalVariables.Selection.GetComponent<MeshRenderer>().material = originalMaterialSelection;
          }
          GlobalVariables.Selection = raycastHit.collider.gameObject;
          if (GlobalVariables.Selection.GetComponent<MeshRenderer>().material != selectionMaterial)
          {
            originalMaterialSelection = originalMaterialHighlight;
            GlobalVariables.Selection.GetComponent<MeshRenderer>().material = selectionMaterial;
          }
          GlobalVariables.Highlight = null;
        }
        else
        {
          if (GlobalVariables.Selection)
          {
            GlobalVariables.Selection.GetComponent<MeshRenderer>().material = originalMaterialSelection;
            GlobalVariables.Selection = null;
          }
        }
      }
    }
  }
}