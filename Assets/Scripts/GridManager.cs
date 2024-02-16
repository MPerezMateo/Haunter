using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
  [SerializeField] private int _width, _depth;
  [SerializeField] private Tile _tilePrefab;
  [SerializeField] private Transform _cam;
  private Dictionary<Vector3, Tile> _tiles;
  void Start()
  {
    // En un futuro, vamos a definir los grids como una lista parametrizada de elementos que va a tener
    // Cargaremos un archivo que contenga la matriz de códigos de tile a cargar, y lo haga.
    // GenerateGrid(new int[9, 16, 2]);
  }
  void GenerateGrid(int[,,] scene)
  {
    for (int i = 0; i < scene.GetLength(0); i++)
    {
      for (int j = 0; j < scene.GetLength(1); j++)
      {
        for (int k = -scene.GetLength(2); k < 0; k++)
        {
          var tile = Instantiate(_tilePrefab, new Vector3(i, j, k), Quaternion.identity);
          tile.name = $"Title {i} {j} {k}";
          // Borrar en un futuro, esto no es así.
          if (k == -1) tile.tag = "Pathable";
        }
      }
    }

    // Aquí deberíamos setear la cámara a un punto óptimo del tablero para visualizar
    //_cam.transform.position = new Vector3(_width * 0.5f - 0.5f, _height * 0.5f - 0.5f, -10);
  }

  public Tile getTileAtPos(Vector3 pos)
  {
    return _tiles.TryGetValue(pos, out var tile) ? tile : null;
  }
}
