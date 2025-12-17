using UnityEngine;

public class SliderResult : MonoBehaviour
{
    public VerticalDiscreteSlider2D slider1;
    public VerticalDiscreteSlider2D slider2;

    void Update()
    {
        int result = slider1.CurrentValue * 10 + slider2.CurrentValue;
        Debug.Log("Результат: " + result);
    }
}
