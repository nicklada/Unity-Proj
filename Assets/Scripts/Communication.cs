using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Communication : MonoBehaviour
{
    private ProgrammManager ProgrammManagerScript;
    private Button button;
   
    void Start()
    {
        ProgrammManagerScript = FindObjectOfType<ProgrammManager>();

        button = GetComponent<Button>();
        button.onClick.AddListener(CommunicationFunction);
    }

    void CommunicationFunction()
    {
        if (ProgrammManagerScript.Communication)
        {
            ProgrammManagerScript.Communication = false;
            GetComponent<Image>().color = Color.white;
        }
        else
        {
            ProgrammManagerScript.Communication = true;
            GetComponent<Image>().color = Color.grey;
        }
    }
               
}
