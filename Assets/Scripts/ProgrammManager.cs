using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ProgrammManager : MonoBehaviour
{
    [Header("Put your planeMarker here")]
    [SerializeField] private GameObject PlaneMarkerPrefab; 

    private ARRaycastManager ARRaycastManagerScript; 

    private Vector2 TouchPosition; 

    public GameObject ObjectToSpawn;

    public bool ChooseObject = false; 
    [Header("Put ScrollView here")]
    public GameObject ScrollView; 

    [SerializeField] private Camera ARCamera; 

    List<ARRaycastHit> hits = new List<ARRaycastHit>(); 

    private GameObject SelectedObject; 

    public bool Moving; 

    void Start()
    {
        ARRaycastManagerScript = FindObjectOfType<ARRaycastManager>(); 

        PlaneMarkerPrefab.SetActive(false); 

        ScrollView.SetActive(false); 
    }

    void Update()
    {
        if (ChooseObject) 
        {
            ShowMarkerAndSetObject(); 
        }

        MoveObject(); 
    }
    void ShowMarkerAndSetObject()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>(); 

        ARRaycastManagerScript.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes); 

        //Появление маркера
        if (hits.Count > 0) 
        {
            PlaneMarkerPrefab.transform.position = hits[0].pose.position; 
            PlaneMarkerPrefab.SetActive(true); 
        }

        //Установка объекта
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) 
        {
            Instantiate(ObjectToSpawn, hits[0].pose.position, ObjectToSpawn.transform.rotation); 
            ChooseObject = false;
            PlaneMarkerPrefab.SetActive(false); 
        }
    }

    void MoveObject() 
    {
        if (Input.touchCount > 0) 
        {
            // Отслеживание места нажатия пальца на экран
            Touch touch = Input.GetTouch(0); 
            TouchPosition = touch.position; 
            
            if (touch.phase == TouchPhase.Began) 
            {
                Ray ray = ARCamera.ScreenPointToRay(touch.position); 
                RaycastHit hitObject; 

                if (Physics.Raycast(ray, out hitObject)) 
                {
                    if (hitObject.collider.CompareTag("UnSelected")) 
                    {
                        hitObject.collider.gameObject.tag = "Selected"; 
                    }
                }
            }

            if (touch.phase == TouchPhase.Moved) 
            {
                if (Moving) 
                {
                    ARRaycastManagerScript.Raycast(TouchPosition, hits, TrackableType.Planes); 
                    SelectedObject = GameObject.FindWithTag("Selected"); 
                    SelectedObject.transform.position = hits[0].pose.position; 
                }
            }

            if (touch.phase == TouchPhase.Ended) 
            {
                if (SelectedObject.CompareTag("Selected")) 
                {
                    SelectedObject.tag = "UnSelected"; 
                }
            }
        }
    }
}

