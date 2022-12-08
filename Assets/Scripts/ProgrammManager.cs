using System.Collections; // Подключение основных расширений (коллекций)
using System.Collections.Generic; // Подключение дополнительных коллекций
using UnityEngine; //Подключение основных функций Unity
using UnityEngine.XR.ARFoundation; // Библиотека для работы ARCore
using UnityEngine.XR.ARSubsystems; // Библиотека для работы ARCore


public class ProgrammManager : MonoBehaviour
{
    [Header("Put your planeMarker here")]
    [SerializeField] private GameObject PlaneMarkerPrefab; // Контейнер PlaneMarkerPrefab для маркера
    private ARRaycastManager ARRaycastManagerScript; // Ссылка на скрипт AR Raycast Manager для получения из него лучей
    private Vector2 TouchPosition; // Создание двумерной переменой TouchPosition
    public GameObject ObjectToSpawn;
    public bool ChooseObject = false; // Создание булевой переменной ChooseObject со значением false, т.е. изначально никакой объект не выбран
    [Header("Put ScrollView here")]
    public GameObject ScrollView; // Создание ячейки под ScrollView
    [SerializeField] private Camera ARCamera; // Создание ячейки под ARCamera
    List<ARRaycastHit> hits = new List<ARRaycastHit>(); // Создание фрейм-контейнера hits, как переменную для уменьшения нагрузки на дальнейший расчет 
    private GameObject SelectedObject; // Создание ячейки SelectedObject под выбранный объект, чтобы в дальнейшем менять его параметры
    public bool Moving; // Создание булевой переменной Moving, которая отвечает за то, перемещает ли пользователь объект или нет (нажата кнопка "Переместить" или нет)
    public bool Rotation; // Создание булевой переменной Rotation, которая отвечает за то, вращает ли пользователь объект или нет (нажата кнопка "Вращение" или нет)
    private Quaternion YRotation; // Создание переменной вращения YRotation (так как вращаем объект только по координате "Y")
    public bool Communication; // Создание булевой переменной Communication, которая отвечает за то, нажата ли кнопка "Диалог" или нет
    [Header("В ячейку перенеси префаб Ассистента")]
    public GameObject VirtualDisplayPrefab; // Создание ячейки под префаб "летающего" виртуального экрана
    private GameObject VirtualDisplay; // Создание ячейки VirtualDisplay под префаб VirtualDisplayPrefab, чтобы в дальнейшем менять его параметры
    [Header("В ячейку перенесите позицию Ассистента")]
    public Transform VirtualDisplayPosition; // Создание ячейки под позицию виртуального экрана VirtualDisplayPosition для того, чтобы в дальнейшем управлять его движением  
    public bool CharacterAnimation = false; // Создание булевой переменной CharacterAnimation со значением false, т.е. изначально у объекта включена анимация "Idol"  

    void Start()
    {
        ARRaycastManagerScript = FindObjectOfType<ARRaycastManager>(); // Поиск в проекте компонента AR Raycast Manager, из которого в дальнейшем можно получить нужную информацию
        PlaneMarkerPrefab.SetActive(false); // Маркер в самом начале не виден
        ScrollView.SetActive(false); // При запуске приложения ScrollView не отображается
    }

    // Update is called once per frame
    void Update()
    {
        if (ChooseObject) // Если пользователь нажал на иконку с объектом, то ChooseObject принимает значение true, соответственно, функция начинает выполняться, на плоскости появляется маркер и пользователю предоставляется возможность установить объект
        {
            ShowMarkerAndSetObject(); // Вызов функции ShowMarkerAndSetObject
        }

        MoveObjectAndRotation(); // Объявление функции MoveObjectAndRotation
        ShowVirtualDisplay(); // Объявление функции ShowVirtualDisplay
        StopVirtualDisplay(); // Объявление функции StopVirtualDisplay

        // Необходимо, чтобы виртуальный экран следовал за пользователем и всегда был в его поле зрения
        // Проверка на дистанцию
        if (CheckDist() >= 0.1f) // Условие выполняется, если дистанция от пользователя до виртуального экрана >= 0.1f (float)
        {
            MoveObjToPos(); // Вызов функции MoveObjToPos при выполнении условия на дистанцию
        }

        VirtualDisplay.transform.LookAt(ARCamera.transform); // Поворот виртуального дисплея в ту сторону, куда смотрит камера (ARCamera) устройства пользователя
    }

    void ShowMarkerAndSetObject()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>(); // Создание фрейм-контейнера hits, в который будут помещаться объекты, попадающиеся на пути следования луча, а именно все пересечения плоскости с лучами
        ARRaycastManagerScript.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes); // Запуск луча из центра экрана (деление ширины и высоты экрана на 2); место хранения информации о пересечении луча с плоскостью - hits; фиксация плоскостей

        //Появление маркера
        if (hits.Count > 0) // Срабатывает в случае, если луч пересек плоскость
        {
            PlaneMarkerPrefab.transform.position = hits[0].pose.position; // Присваиваем позиции маркера значение места, где луч пересекся с плоскостью - hits
            PlaneMarkerPrefab.SetActive(true); // Маркер появляется, становится активным
        }

        //Установка объекта
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) // Условие хотя бы одного нажатия на экран; тестирование фазы начала: произошло ли нажатие на экран
        {
            Instantiate(ObjectToSpawn, hits[0].pose.position, ObjectToSpawn.transform.rotation); // Установка объекта (виртуального ассистента) в место пересечения луча с плоскостью; объект устанавливается той стороной (вращение), которой он стоит в проекте 
            ChooseObject = false;
            PlaneMarkerPrefab.SetActive(false); // После установки объекта пользователем, маркер становится невидимым
        }
    }

    void MoveObjectAndRotation() // Функция, отвечающая за перемещение и вращение объекта
    {
        if (Input.touchCount > 0) // Отслеживание нажатия пальца на экран
        {
            // Отслеживание места нажатия пальца на экран
            Touch touch = Input.GetTouch(0); // Получение информации о касании экрана и её запись в переменную touch
            TouchPosition = touch.position; // Помещение в двумерную переменную TouchPosition координат позиции нажатия пользователя на экран устройства

            if (touch.phase == TouchPhase.Began) // Условие, которое выполняется только в тот момент, когда пользователь коснулся экрана устройства пальцем, т.е. в самом начале
            {
                Ray ray = ARCamera.ScreenPointToRay(touch.position); // Запуск луча, фиксирующего объекты на своём пути, из места касания экрана пальцем пользователя
                RaycastHit hitObject; // Создание контейнера hitObject, хранящего информацию об объектах, которых пересек луч

                if (Physics.Raycast(ray, out hitObject)) // Создание условия, которое выполняется при пересечении лучом объекта 
                {
                    if (hitObject.collider.CompareTag("UnSelected")) // Условие выполняется, если пересеченный лучом объект имеет тег "UnSelected"
                    {
                        hitObject.collider.gameObject.tag = "Selected"; // Невыделенному объекту с тегом UnSelected присваивается тег "Selected" - объект выбран
                    }
                }
            }

            SelectedObject = GameObject.FindWithTag("Selected"); // Помещаем в ячейку SelectedObject объект, у которого есть тег "Selected"   

            if (touch.phase == TouchPhase.Moved && Input.touchCount == 1) // Условие выполняется при движении пальца по экрану (фаза движения) и количестве касаний = 1 (коснулись одним пальцем, а не двумя)
            {
                if (Moving) // Условие выполняется, если значение булевой переменной Moving - true (нажата кнопка "Переместить")
                {
                    ARRaycastManagerScript.Raycast(TouchPosition, hits, TrackableType.Planes); // Запуск лучей из места экрана, куда пользователь коснулся пальцем; полученная информация о пересечении луча с полоскостью помещается в контейнер hits; отслеживание плоскостей                   
                    SelectedObject.transform.position = hits[0].pose.position; // У объекта, помещенного в ячейку SelectedObject (объект с тегом "Selected"), меняем позицию на то место, где луч пересекся с плоскостью
                }

                if (Rotation) // Условие выполняется, если значение булевой переменной Rotation - true (нажата кнопка "Вращение")
                {
                    YRotation = Quaternion.Euler(0f, -touch.deltaPosition.x * 0.1f, 0f); // Переменной YRotation присваивается значение, которое состоит из трех координат вращения (x,y,z). Положение меняется в зависимости от движения пальца по экрану
                    SelectedObject.transform.rotation = YRotation * SelectedObject.transform.rotation; // Меняем позицию вращения выбранного объекта путем её умножения на переменную YRotation, содержащую в себе углы
                }
            }

            if (touch.phase == TouchPhase.Ended) // Условие, выполняющееся в тот момент, когда пользователь отпустил палец от экрана (фаза конца)
            {
                if (SelectedObject.CompareTag("Selected")) // Условие выполняется, если существует объект с тегом "Selected"
                {
                    SelectedObject.tag = "UnSelected"; // Присваиваем объекту с тегом "Selected" обратно тег "UnSelected" - объект не выбран
                }
            }
        }
    }

    void ShowVirtualDisplay() // Функция появления виртуального экрана при нажатии на установленного в пространстве ассистента
    {
        if (Input.touchCount > 0) // Ослеживание нажатия пальца на экран
        {
            Touch touch = Input.GetTouch(0); // Получение информации о касании экрана и её запись в переменную touch
            TouchPosition = touch.position; // Помещение в двумерную переменную TouchPosition координат позиции нажатия пользователя на экран устройства

            if (touch.phase == TouchPhase.Began) // Условие, которое выполняется только в тот момент, когда пользователь коснулся экрана устройства пальцем, т.е. в самом начале 
            {
                Ray ray = ARCamera.ScreenPointToRay(touch.position); // Запуск луча, фиксирующего объекты на своём пути, из места касания экрана пальцем пользователя 
                RaycastHit hitObject; // Создание контейнера hitObject, хранящего информацию об объектах, которых пересек луч

                if (Physics.Raycast(ray, out hitObject)) // Создание условия, которое выполняется при пересечении лучом объекта 
                {
                    if (Communication && CharacterAnimation == false) // Условие выполняется, если значение булевой переменной Communication - true(нажата кнопка "Диалог") и анимация ассистента в режиме "Idol"(для предотвращения появления нескольких виртуальных экранов)
                    {
                        // Появление "летающего" виртуального экрана
                        VirtualDisplay = Instantiate(VirtualDisplayPrefab, ARCamera.transform.position + new Vector3(0, 1f, 0), ARCamera.transform.rotation); // Появление объекта VirtualDisplayPrefab (модели виртуального экрана); позиция "летающего" экрана при его появлении на один метр выше камеры пользователя, чтобы создать эффект анимации (экран опустится сверху в видимую область); вращение объекта
                        CharacterAnimation = true; // Присваиваем переменной CharacterAnimation значение true для того, чтобы анимация ассистента сменилась на "talking" (разговорное состояние)                                                                                                                                      // 
                    }
                }
            }
        }
    }

    public float CheckDist() // Функция, считающая расстояние между камерой и виртуальным экраном
    {
        float dist = Vector3.Distance(VirtualDisplay.transform.position, VirtualDisplayPosition.transform.position); // Запись дистанции между позицией виртуального экрана и позицией камеры (ARCamera) в переменную dist
        return dist; // Возвращается значение dist
    }

    private void MoveObjToPos() // Функция, меняющая позицию виртуального экрана при выполнении условия на дистанцию
    {
        VirtualDisplay.transform.position = Vector3.Lerp(VirtualDisplay.transform.position, VirtualDisplayPosition.position, 1f * Time.deltaTime); // Меняется позиция виртуального экрана путем использования функции сокращения расстояния между двумя объектами (позицией экрана и позицией камеры); с какой скоростью меняется позиция
    }

    private void StopVirtualDisplay() // Функция, которая выполняется при выключении кнопки "Диалог"
    {
        if (Communication == false) // Условие выполняется, если значение булевой переменной Communication - false (пользователь выключает кнопку "Диалог") 
        {
            VirtualDisplay.SetActive(false); // Виртуальный "летающий" экран пропадает с игровой сцены
            CharacterAnimation = false; // Присваиваем переменной CharacterAnimation значение false для того, чтобы анимация ассистента вернулась в исходное состояние "Idol" (состояние покоя)
        }
    }
}
