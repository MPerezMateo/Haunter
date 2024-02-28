using UnityEngine;
using UnityEngine.UI;

public class InformationMenu : MonoBehaviour
{
  [SerializeField] private Camera mainCamera;
  [SerializeField] private Image panel;
  //private GameObject detailObject = null;

  // Start is called before the first frame update
  void Start()
  {
  }

  // Update is called once per frame
  void Update()
  {
    //transform.LookAt(mainCamera.transform);

    //transform.position = mainCamera.transform.position + mainCamera.transform.forward * 4; ;

    // El detalle será dado para el objeto de detalle.
  }
}
