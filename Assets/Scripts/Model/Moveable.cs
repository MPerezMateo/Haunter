using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public abstract class Moveable : MonoBehaviour
{
  public int moveRange;
  public double jumpReach;
  public float moveSpeed;
  public bool highlighted;
  private List<GameObject> posiblePathables;
  private List<float> visited;

  protected void Start()
  {
    // We have to set default values here, in the declaration they do not inherit anything
    moveRange = 7;
    jumpReach = 4.5;
    moveSpeed = 5;
    highlighted = false;
    posiblePathables = new List<GameObject>();
    visited = new List<float>();
  }

  public void selectToMove()
  {
    posiblePathables = optimalPath();
    highLightPaths(posiblePathables, true);
    highlighted = true;
  }

  public void unSelectToMove()
  {
    highLightPaths(posiblePathables, false);
    highlighted = false;
  }
  public void move()
  {
    /* if (Input.GetMouseButtonDown(0) && GlobalVariables.Selection == gameObject && !highlighted)
    {
      posiblePathables = optimalPath();
      highLightPaths(posiblePathables, true);
      highlighted = true;
    } */
    /* else */
    if (Input.GetMouseButtonDown(0) && highlighted)
    {
      List<GameObject> way = setPath(posiblePathables, visited, GlobalVariables.Selection);
      //foreach (var tile in way)
      //  Debug.Log(tile.transform.position);
      unSelectToMove();
      // Move the player
      GlobalVariables.selectable = false;
      StartCoroutine(translatePlayer(way));
      posiblePathables.Clear();
      visited.Clear();
    }
  }

  IEnumerator translatePlayer(List<GameObject> way)
  {
    foreach (GameObject tile in way)
    {
      yield return StartCoroutine(MoveToTarget(tile.transform.position + new Vector3(0, 0, 0.5f * (tile.transform.localScale.z + transform.localScale.z))));
      yield return new WaitForSeconds(0.3f);
    }
    GlobalVariables.selectable = true;
  }

  IEnumerator MoveToTarget(Vector3 targetPosition)
  {
    while (Vector3.Distance(transform.position, targetPosition) > 0f)
    {
      transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
      yield return null;
    }
  }
  private void highLightPaths(List<GameObject> posiblePathables, bool highlight)
  {
    for (int i = 0; i < posiblePathables.Count; i++)
    {
      if (posiblePathables[i] != null)
      {
        Tile tile = posiblePathables[i].GetComponent<Tile>();
        setNumber(posiblePathables[i], i, highlight);
        tile.SetMaterial(highlight ? tile.highLightMaterial : tile.defaultMaterial);
      }
    }
  }

  private void setNumber(GameObject tile, int index, bool highlight)
  {
    TMP_Text textMeshProComponent = tile.GetComponentInChildren<TMP_Text>();
    if (textMeshProComponent)
      textMeshProComponent.text = highlight ? (visited[index] == float.PositiveInfinity ? "F" : visited[index].ToString()) : "";
  }
  /***
  This function is called knowing that the origin and destination are in moverange.
  Wil calculate the optimal path from origin to destination, considering pathable ways and height jump limitation
  **/
  List<GameObject> optimalPath()
  {
    GameObject origin = GameObject.FindGameObjectsWithTag("Pathable").FirstOrDefault(p => p.transform.position.x == transform.position.x && p.transform.position.y == transform.position.y);

    List<GameObject> pathables = GameObject.FindGameObjectsWithTag("Pathable") // Esto debe ser pathable en un futuro
                                       .Where(p => Math.Abs(p.transform.position.x - transform.position.x) + Math.Abs(p.transform.position.y - transform.position.y) <= moveRange)
                                       .OrderBy(p => p.name)
                                       .ToList();

    // Debug.Log(player.transform.position);
    // Debug.Log(origin.transform.position);
    int initialIndex = pathables.IndexOf(origin);
    // ponemos todos a -1 menos en el que esta el personaje
    visited = pathables.Select((e, i) => i == initialIndex ? 0f : float.PositiveInfinity).ToList();
    // Ubicamos los pesos de distancias en nuestra matriz de paso
    setDistance(pathables, visited);
    // Hallamos el camino óptimo sobre los pesos de nuestra matriz de paso
    return pathables;
  }

  bool TestDirection(GameObject origin, List<GameObject> space, List<float> visited, float step, int destIndex, bool advance = false)
  {
    if (destIndex == -1)
      return false;

    float vDistance = Mathf.Abs((origin.transform.position.z + 0.5f * origin.transform.localScale.z) - (space[destIndex].transform.position.z + 0.5f * space[destIndex].transform.localScale.z));

    // La condición es algo rara, pero viene a decir que si "avanzamos" creando un path, entonces buscamos aquel siguiente paso desde desintation hasta origen, por lo que el step decrementa y lo revertimos
    // Si "retocedemos", es porque vamos explorando, y por tanto si avanzamos, tenemos que avanzar porque un camino mejor se abre, luego la casilla de destino debe tener mayor puntuacion que la nuestra 
    return vDistance <= jumpReach &&
           (advance ? visited[destIndex] <= step : visited[destIndex] > step);
  }

  void TestFourDirections(GameObject origin, List<GameObject> space, List<float> visited, float step)
  {
    Vector2[] cross = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };
    int destinationIndex, originIndex = space.IndexOf(origin); ;
    foreach (var direction in cross)
    {
      destinationIndex = space.FindIndex(e => e.transform.position.x == origin.transform.position.x + direction.x && e.transform.position.y == origin.transform.position.y + direction.y);

      if (destinationIndex != -1)
      {
        // Coste de paso de salir de esta casilla. Normalmente es 1, pero podríamos modificarlo.
        int moveCost = space[destinationIndex].GetComponent<Tile>().costPass;
        float vdist = (float)Math.Abs((origin.transform.position.z + 0.5 * origin.transform.localScale.z) - (space[destinationIndex].transform.position.z + 0.5 * space[destinationIndex].transform.localScale.z));
        if (TestDirection(origin, space, visited, visited[originIndex] + moveCost + vdist, destinationIndex, advance: false))
          visited[destinationIndex] = visited[originIndex] + moveCost + vdist;
      }
    }
  }
  void setDistance(List<GameObject> space, List<float> visited)
  {
    for (float step = 1f; step <= moveRange; step++)
    {
      // This loop could be optimized by filtering 
      foreach (GameObject obj in space.Where(e => visited[space.IndexOf(e)] < step && visited[space.IndexOf(e)] >= step - 1))
      {
        TestFourDirections(obj, space, visited, step);
      }
    }
    // Aqui devolver solo los pathables con visited distintos de infinito y menor o igual a moverange
    for (int i = visited.Count - 1; i >= 0; i--)
    {
      if (visited[i] > moveRange)
      {
        space.RemoveAt(i);
        visited.RemoveAt(i);
      }
    }
  }

  List<GameObject> setPath(List<GameObject> space, List<float> visited, GameObject destination)
  {
    List<GameObject> path = new List<GameObject>();
    float step;
    if (!destination)
    {
      Debug.Log("Destination is null. Cannot set path.");
      return path;
    }
    int indexDestination = space.IndexOf(destination);

    if (indexDestination == -1)
    {
      Debug.Log("Can't reach desired location");
      return path;
    }

    path.Add(destination);
    step = visited[indexDestination] - 1f; // menos el coste del cubo, ojo

    Vector2[] cross = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };
    int newPos;
    // No se puede hacer así, se debe consultar a la cruceta
    while (step > 0f)
    {
      int min = int.MaxValue;
      float minValue = float.PositiveInfinity;
      List<GameObject> temp = new List<GameObject>();

      foreach (var dir in cross)
      {
        newPos = space.FindIndex(e =>
         e.transform.position.x == destination.transform.position.x + dir.x && e.transform.position.y == destination.transform.position.y + dir.y);
        if (TestDirection(destination, space, visited, step, newPos, advance: true))
          if (visited[newPos] < minValue)
          {
            min = newPos;
            minValue = visited[newPos];
          }
      }
      //GameObject tempObj = FindClosest(destination, temp);
      path.Add(space[min]);
      destination = space[min];
      step = visited[min];
    }
    path.Reverse();
    return path;
  }

  GameObject FindClosest(GameObject destination, List<GameObject> list)
  {
    if (list.Count == 0)
      return null;

    GameObject closestObject = list[0];
    float minDistance = Vector3.Distance(destination.transform.position, closestObject.transform.position);

    foreach (GameObject obj in list.Skip(1))
    {
      float distance = Vector3.Distance(destination.transform.position, obj.transform.position);

      if (distance < minDistance)
      {
        minDistance = distance;
        closestObject = obj;
      }
    }
    return closestObject;
  }
}