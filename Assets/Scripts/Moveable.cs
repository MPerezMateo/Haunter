using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Moveable : MonoBehaviour
{
  public int movementRange = 6;
  public double jumpReach = .5;
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
                                       .ToList();
    GameObject origin = pathables.Find(p => p.transform.position.x == player.transform.position.x && p.transform.position.y == player.transform.position.y);
    //Debug.Log(player.transform.position);
    //Debug.Log(origin.transform.position);
    int[] visited = pathables.Select(_ => -1).ToArray();
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
    List<GameObject> way = setPath(pathables, visited, destination);
    foreach (var tile in way)
      Debug.Log(tile.transform.position);
  }

  bool TestDirection(GameObject obj, List<GameObject> space, int[] visited, int step, Vector2 direction)
  {
    int destinationIndex = Enumerable.Range(0, space.Count)
                .FirstOrDefault(i => space[i].transform.position.x == obj.transform.position.x + direction.x && space[i].transform.position.y == obj.transform.position.y + direction.y);
    // Aqui poner la cuestión de la altura en el if 
    if (space.FirstOrDefault(e => e.transform.position.x == obj.transform.position.x + direction.x && e.transform.position.y == obj.transform.position.y + direction.y) != null
        && visited[destinationIndex] == step) // && Math.Abs(transform.position.z - space[destinationIndex].transform.position.z) <= jumpReach
      return true;
    return false;
  }

  void TestFourDirections(GameObject obj, List<GameObject> space, int[] visited, int step)
  {
    Vector2[] cross = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
    int destinationIndex;
    foreach (var direction in cross)
    {
      if (TestDirection(obj, space, visited, -1, direction))
      {
        destinationIndex = Enumerable.Range(0, space.Count)
                .FirstOrDefault(i => space[i].transform.position.x == obj.transform.position.x + direction.x && space[i].transform.position.y == obj.transform.position.y + direction.y);
        visited[destinationIndex] = step;
      }
    }
  }
  void setDistance(List<GameObject> space, int[] visited)
  {
    int index;
    for (int step = 1; step < space.Count; step++)
    {
      foreach (GameObject obj in space)
      {
        index = Enumerable.Range(0, space.Count)
              .FirstOrDefault(i => space[i].transform.position.x == obj.transform.position.x && space[i].transform.position.y == obj.transform.position.y);
        if (visited[index] == step - 1)
          TestFourDirections(obj, space, visited, step);
      }
    }
  }

  List<GameObject> setPath(List<GameObject> space, int[] visited, GameObject destination)
  {
    List<GameObject> path = new List<GameObject>();
    List<GameObject> temp = new List<GameObject>();
    int step;
    int indexDestination = Enumerable.Range(0, space.Count)
              .FirstOrDefault(i => space[i].transform.position.x == destination.transform.position.x && space[i].transform.position.y == destination.transform.position.y);

    if (visited[indexDestination] > 0)
    {
      path.Add(destination);
      step = visited[indexDestination] - 1;
    }
    else
    {
      Debug.Log("Can't reach desired location");
      return null;
    }
    Vector2[] cross = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
    int destinationIndex;
    for (int i = step; step > -1; step--)
    {
      foreach (var dir in cross)
      {
        destinationIndex = Enumerable.Range(0, space.Count)
                .FirstOrDefault(i => space[i].transform.position.x == destination.transform.position.x + dir.x && space[i].transform.position.y == destination.transform.position.y + dir.y);
        // Esta parte está mal
        if (TestDirection(destination, space, visited, step, dir))
          temp.Add(space[destinationIndex]);
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
    int indexNumber = 0;
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
    return list[indexNumber];
  }
}
