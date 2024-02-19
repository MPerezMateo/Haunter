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
  private bool highlighted;
  private List<GameObject> posiblePathables;
  private List<float> visited;

  protected void Start()
  {
    // We have to set default values here, in the declaration they do not inherit anything
    moveRange = 7;
    jumpReach = .5;
    moveSpeed = 5;
    highlighted = false;
    posiblePathables = new List<GameObject>();
    visited = new List<float>();
  }
  protected void move()
  {
    if (Input.GetMouseButtonDown(0) && GlobalVariables.Selection == gameObject && !highlighted)
    {
      posiblePathables = optimalPath();
      highLightPaths(posiblePathables, true);
      highlighted = true;
    }
    else if (Input.GetMouseButtonDown(0) && highlighted)
    {
      List<GameObject> way = setPath(posiblePathables, visited, GlobalVariables.Selection);
      //foreach (var tile in way)
      //  Debug.Log(tile.transform.position);
      highLightPaths(posiblePathables, false);
      highlighted = false;
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
    List<GameObject> pathables = GameObject.FindGameObjectsWithTag("Pathable") // Esto debe ser pathable en un futuro
                                       .Where(p => Math.Abs(p.transform.position.x - transform.position.x) + Math.Abs(p.transform.position.y - transform.position.y) <= moveRange)
                                       .OrderBy(p => p.name)
                                       .ToList();

    GameObject origin = pathables.Find(p => p.transform.position.x == transform.position.x && p.transform.position.y == transform.position.y);
    // Debug.Log(player.transform.position);
    // Debug.Log(origin.transform.position);
    int initialIndex = pathables.FindIndex(e => e.transform.position.x == origin.transform.position.x && e.transform.position.y == origin.transform.position.y);
    // ponemos todos a -1 menos en el que esta el personaje
    visited = pathables.Select((e, i) => i == initialIndex ? 0f : float.PositiveInfinity).ToList();
    if (!pathables.Contains(origin))
    {
      Debug.Log("Error, destination or origin not in tile pool");
      return new List<GameObject>();
    }
    // Ubicamos los pesos de distancias en nuestra matriz de paso
    setDistance(pathables, visited);
    // Hallamos el camino óptimo sobre los pesos de nuestra matriz de paso
    return pathables;
  }

  bool TestDirection(GameObject origin, List<GameObject> space, List<float> visited, float step, int destIndex, bool advance = false)
  {
    //Debug.Log($"Distance between {origin} and {space[destIndex]} is {Math.Abs((origin.transform.position.z + 0.5 * origin.transform.localScale.z) - (space[destIndex].transform.position.z + 0.5 * space[destIndex].transform.localScale.z))}");
    if (destIndex != -1 && (advance ? visited[destIndex] <= step : visited[destIndex] > step)
         && Math.Abs((origin.transform.position.z + 0.5 * origin.transform.localScale.z) - (space[destIndex].transform.position.z + 0.5 * space[destIndex].transform.localScale.z)) <= jumpReach)
      return true;
    return false;
  }

  void TestFourDirections(GameObject origin, List<GameObject> space, List<float> visited, float step)
  {
    Vector2[] cross = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
    int destinationIndex, originIndex;
    foreach (var direction in cross)
    {
      destinationIndex = space.FindIndex(e => e.transform.position.x == origin.transform.position.x + direction.x && e.transform.position.y == origin.transform.position.y + direction.y);
      originIndex = space.FindIndex(e => e == origin);

      if (destinationIndex != -1)
      {
        float vdist = (float)Math.Abs((origin.transform.position.z + 0.5 * origin.transform.localScale.z) - (space[destinationIndex].transform.position.z + 0.5 * space[destinationIndex].transform.localScale.z));
        if (TestDirection(origin, space, visited, visited[originIndex] + 1 + vdist, destinationIndex, advance: false))
        {
          // Aquí, al step hay que agregarle la penalización por altura
          visited[destinationIndex] = visited[originIndex] + 1 + vdist;
        }
      }
    }
  }
  void setDistance(List<GameObject> space, List<float> visited)
  {
    int index;
    float step = 1f;
    while (step < space.Count)
    {
      foreach (GameObject obj in space)
      {
        // We find the index of our object
        index = space.FindIndex(e => e == obj);
        // if (visited[index] > -1 && visited[index] < step)
        if (visited[index] < step)
          TestFourDirections(obj, space, visited, step);
      }
      step++;
    }

    // Aqui devolver solo los pathables con visited distintos de -1
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
    List<GameObject> temp = new List<GameObject>();
    float step;
    if (!destination)
      return path;
    int indexDestination = space.FindIndex(e => e.transform.position.x == destination.transform.position.x && e.transform.position.y == destination.transform.position.y);

    if (indexDestination != -1)
    {
      path.Add(destination);
      step = visited[indexDestination] - 1f;
    }
    else
    {
      Debug.Log("Can't reach desired location");
      return path;
    }
    Vector2[] cross = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
    int newPos;
    for (float i = step; step > 0f; step--)
    {
      foreach (var dir in cross)
      {
        newPos = space.FindIndex(e =>
         e.transform.position.x == destination.transform.position.x + dir.x && e.transform.position.y == destination.transform.position.y + dir.y);
        if (TestDirection(destination, space, visited, step, newPos, advance: true))
          temp.Add(space[newPos]);
      }
      GameObject tempObj = FindClosest(destination, temp);
      path.Add(tempObj);
      destination = tempObj;
      temp.Clear();
    }
    path.Reverse();
    return path;
  }

  GameObject FindClosest(GameObject destination, List<GameObject> list)
  {
    float currentDistance = float.PositiveInfinity;
    int indexNumber = -1;
    float dist;
    for (int i = 0; i < list.Count; i++)
    {
      dist = Vector3.Distance(destination.transform.position, list[i].transform.position);
      if (dist < currentDistance)
      {
        currentDistance = dist;
        indexNumber = i;
      }
    }
    return indexNumber != -1 ? list[indexNumber] : null;
  }
}
