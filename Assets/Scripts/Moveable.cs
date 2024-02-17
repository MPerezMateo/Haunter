using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Moveable : MonoBehaviour
{
  public int movementRange;
  public double jumpReach;

  protected void Start()
  {
    // We have to set default values here, in the declaration they do not inherit anything
    movementRange = 7;
    jumpReach = .5;
  }
  protected void move()
  {
    if (Input.GetMouseButtonDown(0) && GlobalVariables.selection == gameObject && GlobalVariables.highlight != null && GlobalVariables.highlight != gameObject && GlobalVariables.highlight.tag == "Pathable")
    {
      //Debug.Log(GlobalVariables.selection.transform.position);
      //Debug.Log(GlobalVariables.highlight.transform.position);
      //transform.position = GlobalVariables.highlight.transform.position + new Vector3(.0f, .0f, 0.5f * (transform.localScale.z + GlobalVariables.highlight.transform.localScale.z));
      optimalPath(gameObject, GlobalVariables.highlight);
      // una vez determinado el camino óptimo, hay que desplazar a una velocidad lineal al "movible"
    }
  }

  /***
  This function is called knowing that the origin and destination are in moverange.
  Wil calculate the optimal path from origin to destination, considering pathable ways and height jump limitation
  **/
  void optimalPath(GameObject player, GameObject destination)
  {
    List<GameObject> pathables = GameObject.FindGameObjectsWithTag("Pathable") // Esto debe ser pathable en un futuro
                                       .Where(p => Math.Abs(p.transform.position.x - transform.position.x) + Math.Abs(p.transform.position.y - transform.position.y) <= movementRange)
                                       .OrderBy(p => p.name)
                                       .ToList();

    GameObject origin = pathables.Find(p => p.transform.position.x == player.transform.position.x && p.transform.position.y == player.transform.position.y);
    //Debug.Log(player.transform.position);
    //Debug.Log(origin.transform.position);
    List<int> visited = pathables.Select(_ => -1).ToList();
    int initialIndex = Enumerable.Range(0, pathables.Count)
                .FirstOrDefault(i => pathables[i].transform.position.x == origin.transform.position.x && pathables[i].transform.position.y == origin.transform.position.y);
    visited[initialIndex] = 0;

    if (!pathables.Contains(destination) || !pathables.Contains(origin))
    {
      Debug.Log("Error, destination or origin not in tile pool");
      return;
    }
    // Ubicamos los pesos de distancias en nuestra matriz de paso
    setDistance(pathables, visited);
    // Hallamos el camino óptimo sobre los pesos de nuestra matriz de paso
    if (!pathables.Contains(destination))
    {
      Debug.Log("Error, destination or origin not reachable");
      return;
    }
    List<GameObject> way = setPath(pathables, visited, destination);
    foreach (var tile in way)
      Debug.Log(tile.transform.position);
  }

  bool TestDirection(GameObject origin, List<GameObject> space, List<int> visited, int step, int destIndex)
  {
    //Debug.Log($"Distance between {origin} and {space[destIndex]} is {Math.Abs((origin.transform.position.z + 0.5 * origin.transform.localScale.z) - (space[destIndex].transform.position.z + 0.5 * space[destIndex].transform.localScale.z))}");
    if (destIndex != -1 && visited[destIndex] == step
         && Math.Abs((origin.transform.position.z + 0.5 * origin.transform.localScale.z) - (space[destIndex].transform.position.z + 0.5 * space[destIndex].transform.localScale.z)) <= jumpReach)
      return true;
    return false;
  }

  void TestFourDirections(GameObject origin, List<GameObject> space, List<int> visited, int step)
  {
    Vector2[] cross = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
    int destinationIndex;
    foreach (var direction in cross)
    {
      destinationIndex = space.FindIndex(e => e.transform.position.x == origin.transform.position.x + direction.x && e.transform.position.y == origin.transform.position.y + direction.y);
      if (TestDirection(origin, space, visited, -1, destinationIndex))
      {
        visited[destinationIndex] = step;
      }
    }
  }
  void setDistance(List<GameObject> space, List<int> visited)
  {
    int index;
    for (int step = 1; step < space.Count; step++)
    {
      foreach (GameObject obj in space)
      {
        // We find the index of our object
        index = space.FindIndex(e => e.transform.position.x == obj.transform.position.x && e.transform.position.y == obj.transform.position.y);
        if (visited[index] == step - 1)
          TestFourDirections(obj, space, visited, step);
      }
    }

    // Aqui devolver todos los pathables distintos de -1
    // En un futuro, al seleccionar a nuestro movible, debemos resaltar las áreas posibles.

    for (int i = visited.Count - 1; i >= 0; i--)
    {
      if (visited[i] == -1)
      {
        space.RemoveAt(i);
        visited.RemoveAt(i);
      }
    }
  }

  List<GameObject> setPath(List<GameObject> space, List<int> visited, GameObject destination)
  {
    List<GameObject> path = new List<GameObject>();
    List<GameObject> temp = new List<GameObject>();
    int step;
    int indexDestination = space.FindIndex(e => e.transform.position.x == destination.transform.position.x && e.transform.position.y == destination.transform.position.y);

    if (visited[indexDestination] > 0 && visited[indexDestination] <= movementRange)
    {
      path.Add(destination);
      step = visited[indexDestination] - 1;
    }
    else
    {
      Debug.Log("Can't reach desired location");
      return path;
    }
    Vector2[] cross = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
    int newPos;
    for (int i = step; step > -1; step--)
    {
      foreach (var dir in cross)
      {
        newPos = space.FindIndex(e =>
         e.transform.position.x == destination.transform.position.x + dir.x && e.transform.position.y == destination.transform.position.y + dir.y);
        if (TestDirection(destination, space, visited, step, newPos))
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
