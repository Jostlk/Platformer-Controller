using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriggersController : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    public Order order;

    public enum Order
    {
        first = 0, second, third, fourth, fifth
    };


    private void Update()
    {
    
    }
   
    private void OnTriggerEnter(Collider collider)
    {
        TextSwitch();
    }

    private void TextSwitch()
    {
        switch (order)
        {
            case Order.first:
                textMeshProUGUI.SetText("Используйте Shift для ускорения");
                break;
            case Order.second:
                textMeshProUGUI.SetText("Используйте Ctrl для рывка");
                break;
            case Order.third:
                textMeshProUGUI.SetText("W - карабкаться вверх, S - карабкаться вниз");
                break;
            case Order.fourth:
                textMeshProUGUI.SetText("Используйте Ctrl для рывка");
                break;
            case Order.fifth:
                textMeshProUGUI.SetText("Так же мы добавили Coyote Time и Jump Buffering ");
                break;
        }

    }

}
