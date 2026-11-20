using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.UI.CanvasScaler;
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
    GameObject typeDisplay;



    [Header("Unit")]
    // keyboard stuff
    public Material rangeMat;
    Key[] digits =
    {
        Key.Digit1, Key.Digit2, Key.Digit3,
        Key.Digit4, Key.Digit5, Key.Digit6,
        Key.Digit7, Key.Digit8, Key.Digit9

    };
    
    int currentUnit = DESELECTED;
    GameObject phantom;
    GameObject lastSelected;
    GameObject currRange;


    public LayerMask hitMask;
    [Header("Constant")]
    const int DESELECTED = -1;
    
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

        // ===================================
        // check if click on unit
        // ===================================
        if (Mouse.current.leftButton.wasPressedThisFrame) 
        {
            isUnit();
        }

        // ===================================
        //          CAM CONTROL
        // ===================================
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

        Vector3 raw = inp.ReadValue<Vector3>();
        Vector3 move = Quaternion.Euler(0f, yaw, 0f) * raw;


        tarPos += move.normalized * speed;
        tarPos.x = Mathf.Clamp(tarPos.x,-24f,24f);
        tarPos.z = Mathf.Clamp(tarPos.z, -24f, 24f);

        // ===================================
        //  PLACING UNIT
        // ===================================

        isNumKeyPressed();
        if (phantom)
        {
            phantomToMouse();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && currentUnit != DESELECTED)
        {
            print(currentUnit);
            rayCastandPlace(currentUnit);
        }
    }

    private void LateUpdate()
    {
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, tarPos, ref v, 0.25f);
        cam.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }


    // ===================================
    //      CLICK ON UNIT
    // ===================================

    bool isUnit() 
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) )
        {
            
            if (hit.collider.gameObject.CompareTag("Towers"))
            {
                display.SetActive(true);

                var script = hit.collider.gameObject.GetComponent<UnitStat>();
                var (unitName, dmg, spa, rng, plc, type) = script.getStat();

                nameDisplay.GetComponent<TMP_Text>().text = unitName;
                statDisplay.transform.GetChild(0).GetComponent<TMP_Text>().text = $"DMG: {dmg}";
                statDisplay.transform.GetChild(1).GetComponent<TMP_Text>().text = $"SPA: {spa}";
                statDisplay.transform.GetChild(2).GetComponent<TMP_Text>().text = $"RNG: {rng}";
                statDisplay.transform.GetChild(3).transform.GetChild(0).GetComponent<TMP_Text>().text = type;
                GameObject currentSelected = hit.collider.gameObject;
                currRange = hit.collider.gameObject.transform.GetChild(0).gameObject;

                if (lastSelected && lastSelected != currentSelected) 
                {
                    lastSelected.transform.GetChild(0).gameObject.SetActive(false);
                }

                currRange.SetActive(true);
                lastSelected = hit.collider.gameObject;
                return true;
            }

            if (EventSystem.current.IsPointerOverGameObject()) 
            {
                
                return true;
            }
        }

        if (currRange) { currRange.SetActive(false); }
        
        display.SetActive(false);
        return false;
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
            if (child.name == "Type") { typeDisplay = child.gameObject; }
        }
    }



    // ===================================
    //      Unit placement helper
    // ===================================
    void rayCastandPlace(int index)
    {

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10000f, hitMask))
        {
            //GameObject unit = hit.collider.gameObject;

            if (hit.collider.gameObject.CompareTag("PlayableArea"))
            {
                Vector3 hitPos = hit.point;
                Vector3 yOffset = new Vector3(0f, phantom.GetComponent<MeshRenderer>().bounds.size.y / 2, 0f);
                GameObject tower = Instantiate(unitList[index].unitModel, hitPos + yOffset, Quaternion.identity);
                tower.GetComponent<UnitStat>().initializeUnit(unitList[index]);
                tower.tag = "Towers";

                createRange(tower.transform, tower.GetComponent<UnitStat>().range, yOffset);
                tower.transform.GetChild(0).gameObject.SetActive(false);
                currentUnit = DESELECTED;
                Destroy(phantom);
            }
        }
    }

    void createRange(Transform currUnit, float range, Vector3 yOff) 
    {
        GameObject rangeObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rangeObj.name = "range disc";
        Destroy(rangeObj.GetComponent<Collider>());
        rangeObj.transform.SetParent(currUnit, false);
        rangeObj.transform.localPosition = new Vector3(0f, 0.02f, 0f) - yOff;

        Vector3 s = currUnit.lossyScale;
        rangeObj.transform.localScale = new Vector3(
            range * 2f / s.x,
            0.01f / s.y,
            range * 2f / s.z);

        rangeObj.GetComponent<Renderer>().material = rangeMat;
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

    void phantomToMouse() 
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000f, hitMask))
        {
            //GameObject unit = hit.collider.gameObject;
            if (!hit.collider.gameObject.CompareTag("Phantom"))
            {
                Vector3 hitPos = hit.point;
                Vector3 yOffset = new Vector3(0f, phantom.GetComponent<MeshRenderer>().bounds.size.y/2, 0f);
                phantom.transform.position = hitPos + yOffset;
                phantom.transform.GetChild(0).gameObject.SetActive(true);
            }


        }
    }


    bool isNumKeyPressed() 
    {
        for (int i = 0; i < digits.Length; i++) 
        {
            if (Keyboard.current[digits[i]].wasPressedThisFrame) 
            {
                if (currentUnit == i) { currentUnit = DESELECTED; Destroy(phantom); return true;  }
                if (phantom) { Destroy(phantom); }
                
                currentUnit = i;

                phantom = Instantiate(unitList[i].unitModel);
                Vector3 yOffset = new Vector3(0f, phantom.GetComponent<MeshRenderer>().bounds.size.y / 2, 0f);
                createRange(phantom.transform, unitList[i].range, yOffset);
                


                return true;
            }
        }
        return false;

    }

    public void closeRangeLast() 
    {
        lastSelected.transform.GetChild(0).gameObject.SetActive(false);
    }
}
