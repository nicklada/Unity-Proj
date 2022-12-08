using System.Collections; // Подключение основных расширений (коллекций)
using System.Collections.Generic; // Подключение дополнительных коллекций
using UnityEngine; //Подключение основных функций Unity

public class CharacterAnimation : MonoBehaviour // Класс CharacterAnimatio (название скрипта) наследует класс MonoBehaviour
{
    private ProgrammManager ProgrammManagerScript; // Создание переменной под скрипт ProgrammManager, чтобы в дальнейшем на него ссылаться
    private Animator anim; // Создание переменной anim, которая будет являться ссылкой на компонент аниматора (Animator) 

    void Start()
    {
        ProgrammManagerScript = FindObjectOfType<ProgrammManager>(); // Создание переменной под скрипт ProgrammManager, чтобы в дальнейшем на него ссылаться
        anim = GetComponent<Animator>(); // Поиск и запись найденного компонента Animator в переменную anim (запись действий из аниматора в переменую)
    }

    void Update()
    {
        if (ProgrammManagerScript.CharacterAnimation) // Условие выполняется, если значение булевой переменной CharacterAnimation - true ("летающий" виртуальный экран появляется на сцене, нажата кнопка "Диалог")
        {
            anim.SetBool("talking", true); // Срабатывает bool "talking", и ассистент сменяет анимацию покоя "Idol" на "разговорное состояние - talking/talking1"
        }
        else // Выполняется, если значение булевой переменной CharacterAnimation - false (кнопка "Диалог" не нажата)
        {
            anim.SetBool("talking", false); // Анимация объекта возвращается в исходное состояние "Idol" из состояния "talking/talking1"
        }
    }
}
