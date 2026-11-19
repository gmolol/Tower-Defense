using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    [Header("Movement(Cam)")]
    public InputAction inp;
    public Vector3 move;
    private float speed = 0.2f;



    [Header("Map Generator")]
    public PathGenerator pathGenerator;

    [Header("Camera")]
    public Camera cam;
    private float zoom;
    private float velocity = 0.5f;
    private Vector3 v = Vector3.zero;
    Vector3 tarPos;
    public float pitch, yaw, targetYaw, yawVel;

    [Header("Hand Manager")]
    public HandManager handManager;
    [Header("Deck Manager")]
    public DeckManager deckManager;

    [Header("Unit List")]
    public UnitList unitObject;
    List<UnitData> unitList;


    [Header("Display")]
    public GameObject display;
    GameObject statDisplay;
    GameObject unitDisplay;
    GameObject nameDisplay;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inp.Enable();
        zoom = cam.orthographicSize;
        tarPos = cam.transform.position;

        Vector3 e = cam.transform.eulerAngles;
        pitch = e.x;
        yaw = targetYaw = e.y;

        unitList = unitObject.unit_list;


        initializeDisplay();
    }

    
    void Update()
    {

        // WHEN CLICKED ON UNIT
        if (Mouse.current.leftButton.wasPressedThisFrame) 
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) 
            {
                //GameObject unit = hit.collider.gameObject;
                if (hit.collider.gameObject.CompareTag("Towers")) 
                {
                    Debug.Log("Clicked on unit");
                }
            }
        }

        Vector2 scroll = Mouse.current.scroll.ReadValue();
        if (scroll.y > 0)
        {
            zoom -= 1;
        }
        else if (scroll.y < 0)
        {
            zoom += 1;
        }
        //print(scroll);

        zoom = Mathf.Clamp(zoom, 4f, 17.4f);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, 0.25f);
        //move = inp.ReadValue<Vector3>().normalized;




        if (Keyboard.current.qKey.isPressed) 
        {
            targetYaw += 0.2f;
        }
        if (Keyboard.current.eKey.isPressed)
        {
            targetYaw -= 0.2f;
        }
        yaw = Mathf.SmoothDampAngle(yaw, targetYaw, ref yawVel, 0.25f);
        //tarPos +=  (move*speed)/2;



        Vector3 raw = inp.ReadValue<Vector3>();
        Vector3 move = Quaternion.Euler(0f, yaw, 0f) * raw;


        tarPos += move.normalized * speed;
        tarPos.x = Mathf.Clamp(tarPos.x,-24f,24f);
        tarPos.z = Mathf.Clamp(tarPos.z, -24f, 24f);







        //==================
        //  PLACING UNIT
        //==================
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {

            rayCastandPlace();

            //print(unitInfoDebug(0));
        }

    }

    private void LateUpdate()
    {
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, tarPos, ref v, 0.25f);
        cam.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }


    // ===================================
    //      Display Helper
    // ===================================

    void initializeDisplay() 
    {
        foreach (Transform child in display.transform) 
        {
            if (child.name == "Name") { nameDisplay = child.gameObject; }
            if (child.name == "Display") { unitDisplay = child.gameObject; }
            if (child.name == "Stat") { statDisplay = child.gameObject; }
        }
    }



    // ===================================
    //      Unit placement helper
    // ===================================
    void rayCastandPlace()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //GameObject unit = hit.collider.gameObject;

            if (hit.collider.gameObject.CompareTag("PlayableArea"))
            {
                Vector3 hitPos = hit.point;
                GameObject tower = Instantiate(unitList[0].unitModel, hitPos, Quaternion.identity);
                tower.GetComponent<UnitStat>().initializeUnit(unitList[0]);
            }
        }
    }

    string unitInfoDebug(int a)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine();
        sb.Append("Name: " + unitList[a].unitName);
        sb.AppendLine();
        sb.Append($"Type: {unitList[a].type}");
        sb.AppendLine();
        sb.Append($"Dmg: {unitList[a].damage} \t Spa: {unitList[a].spa} \t Rng: {unitList[a].range}");

        return sb.ToString();

    }
}
