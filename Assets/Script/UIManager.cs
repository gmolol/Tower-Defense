using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject statWindow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void closeStatWindow() 
    { 
        statWindow.SetActive(false);
    }
}
