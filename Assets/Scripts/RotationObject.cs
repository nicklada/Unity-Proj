using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class RotationObject : MonoBehaviour
{
    private Button button; 
    private ProgrammManager ProgrammManagerScript; 

    void Start()
    {
        ProgrammManagerScript = FindObjectOfType<ProgrammManager>(); 

        button = GetComponent<Button>(); 
        button.onClick.AddListener(RotationObjectFunction); 

    }

    void RotationObjectFunction() 
    {
        if (ProgrammManagerScript.Rotation) 
        {
            ProgrammManagerScript.Rotation = false; 
            GetComponent<Image>().color = Color.white; 
        }
        else 
        {
            ProgrammManagerScript.Rotation = true; 
            GetComponent<Image>().color = Color.grey; 
        }
    }
}

