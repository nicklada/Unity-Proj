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
    public bool Rotation; 
    private Quaternion YRotation; 
    public bool Communication; 
    [Header("В ячейку перенеси префаб Ассистента")]
    public GameObject VirtualDisplayPrefab; 
    private GameObject VirtualDisplay; 
    [Header("В ячейку перенесите позицию Ассистента")]
    public Transform VirtualDisplayPosition; 
    public bool CharacterAnimation = false;

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
        MoveObjectAndRotation(); 
        ShowVirtualDisplay(); 
        StopVirtualDisplay(); 

        // Необходимо, чтобы виртуальный экран следовал за пользователем и всегда был в его поле зрения
        // Проверка на дистанцию
        if (CheckDist() >= 0.1f) 
        {
            MoveObjToPos(); 
        }

        VirtualDisplay.transform.LookAt(ARCamera.transform); 
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

    void MoveObjectAndRotation() 
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
            SelectedObject = GameObject.FindWithTag("Selected"); 
            if (touch.phase == TouchPhase.Moved && Input.touchCount == 1) 
            {
                if (Moving) 
                {
                    ARRaycastManagerScript.Raycast(TouchPosition, hits, TrackableType.Planes); 
                    SelectedObject.transform.position = hits[0].pose.position; 
                }
                if (Rotation) 
                {
                    YRotation = Quaternion.Euler(0f, -touch.deltaPosition.x * 0.1f, 0f); 
                    SelectedObject.transform.rotation = YRotation * SelectedObject.transform.rotation; 
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

    void ShowVirtualDisplay() 
    {
        if (Input.touchCount > 0) 
        {
            Touch touch = Input.GetTouch(0); 
            TouchPosition = touch.position; 
            if (touch.phase == TouchPhase.Began) 
            {
                Ray ray = ARCamera.ScreenPointToRay(touch.position); 
                RaycastHit hitObject; 
                if (Physics.Raycast(ray, out hitObject)) 
                {
                    if (Communication && CharacterAnimation == false) 
                    {
                        // Появление "летающего" виртуального экрана
                        VirtualDisplay = Instantiate(VirtualDisplayPrefab, ARCamera.transform.position + new Vector3(0, 1f, 0), ARCamera.transform.rotation); 
                        CharacterAnimation = true;
                    }
                }
            }
        }
    }
   
 public float CheckDist() 
    {
        float dist = Vector3.Distance(VirtualDisplay.transform.position, VirtualDisplayPosition.transform.position); 
        return dist; 
    }
    
private void MoveObjToPos() 
    {
        VirtualDisplay.transform.position = Vector3.Lerp(VirtualDisplay.transform.position, VirtualDisplayPosition.position, 1f * Time.deltaTime); 
    }

    private void StopVirtualDisplay() 
    {
        if (Communication == false) 
        {
            VirtualDisplay.SetActive(false);
            CharacterAnimation = false; 
        }
    }
}

