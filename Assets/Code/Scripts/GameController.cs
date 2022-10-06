using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            // Application.Quit();
            Debug.Log("You Win!");
        }     
    }
}
