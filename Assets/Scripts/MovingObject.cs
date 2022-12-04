using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class MovingObject : MonoBehaviour
{
    private Button button; 
    private ProgrammManager ProgrammManagerScript; 
    
    void Start()
    {
        ProgrammManagerScript = FindObjectOfType<ProgrammManager>(); 
        
        button = GetComponent<Button>(); 
        button.onClick.AddListener(MovingObjectFunction); 
    }

    void MovingObjectFunction() 
    {
        if (ProgrammManagerScript.Moving) 
        {
            ProgrammManagerScript.Moving = false; 
            GetComponent<Image>().color = Color.white; 
        }
        else 
        {
            ProgrammManagerScript.Moving = true; 
            GetComponent<Image>().color = Color.grey; 
        }
    }
}

