using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddObject : MonoBehaviour
{
    private ProgrammManager ProgrammManagerScript; 

    private Button button; 

    void Start()
    {
        ProgrammManagerScript = FindObjectOfType<ProgrammManager>(); 
        button = GetComponent<Button>(); 
        button.onClick.AddListener(AddObjectFunction); 

    }

    void AddObjectFunction()
    {
        ProgrammManagerScript.ScrollView.SetActive(true); 
    }
}

