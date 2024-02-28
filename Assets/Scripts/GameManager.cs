using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{

  [SerializeField] private GameObject infoMenu;
  private List<AttackerAndDecision> myTeam;
  private List<AttackerAndDecision> enemyTeam;

  private List<AttackerAndDecision> agents;

  // Start is called before the first frame update
  void Start()
  {
    infoMenu.SetActive(false);

    // We set all agents in our system as the sum of teams in the game
    myTeam = AddTeamMembers("Player");
    // TODO multi team optionalities.
    enemyTeam = AddTeamMembers("Enemy");
    agents = (myTeam ?? new List<AttackerAndDecision>())
              .Concat(enemyTeam ?? new List<AttackerAndDecision>())
              .ToList();

    ManageTurn();
    // We are gonna allow to move the camera using keys,
    // Camera pos must be parametrized based on main grid layout, which should be scanned after render
    // Based on the dimensions of the scan, we will place the angle of the camera
    // For the time being, we will hardcode the coords of the camera
    // Original pos is x= 8 y = 0 z = 5 xr= 0 yr = 116.44 zr= 90
    // Will rotate to other positions, eg
    // x -8 yr -116.44 zr = -90
  }

  // Update is called once per frame
  void Update()
  {
    // moveInElipse(MainCamera);
    if (GlobalVariables.Selection != null && GlobalVariables.CurrentAttacker != null)
      infoMenu.SetActive(GlobalVariables.Selection.GetComponent<Attacker>() == GlobalVariables.CurrentAttacker.attacker);
    else
      infoMenu.SetActive(false);
    // Gestión de los turnos.
  }

  void ManageTurn()
  {
    foreach (AttackerAndDecision attacker in myTeam)
    {
      attacker.decisions.Clear();
    }
    foreach (AttackerAndDecision enemy in enemyTeam)
    {
      enemy.decisions.Clear();
    }
    // Al empezar un turno, lo primero que vamos a hacer es ordernar nuestros atacantes por su nivel de velocidad.
    agents = agents.OrderByDescending(a => a.attacker.speed).ToList();
    // A continuación, les vamos a ir dando paso, dejándoles actuar
    foreach (AttackerAndDecision agent in agents)
    {
      GlobalVariables.CurrentAttacker = agent;
      // Aquí, debemos esperar a que el comando de "espera" esté completado antes de dar paso al siguiente elemento.
    }
  }

  private List<AttackerAndDecision> AddTeamMembers(string tag)
  {
    return GameObject.FindGameObjectsWithTag(tag)
      .Select(member => member.GetComponent<Attacker>())
      .OfType<Attacker>()
      .Select(attacker => new AttackerAndDecision { attacker = attacker, decisions = new List<Decision>() })
      .ToList() ?? new List<AttackerAndDecision>();
  }
}
