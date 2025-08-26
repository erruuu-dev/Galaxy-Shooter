using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  

public class Main_Menu_Script : MonoBehaviour
{

    public void startSinglePlayer()
    {
        SceneManager.LoadScene(1); 
    }    
    
    public void startCoOp()
    {
        SceneManager.LoadScene(2);
    }
}
