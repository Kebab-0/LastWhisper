using UnityEngine;

public class SliderResult : MonoBehaviour
{
    public VerticalDiscreteSlider2D slider1;
    public VerticalDiscreteSlider2D slider2;
    public Detector detector;
    [SerializeField] private Sensors sensors;
    public bool playOnChangeOnly = true;

    private int lastResult = -1;

    void Update()
    {
        if (slider1 == null || slider2 == null)
            return;

        int result = slider1.CurrentValue * 10 + slider2.CurrentValue;

        if (!playOnChangeOnly || result != lastResult)
        {
            lastResult = result;
            TryPlaySensorSound(result);
            HighlightSelectedSensor(result);
        }
    }

    private void TryPlaySensorSound(int result)
    {
        if (detector == null)
            return;

        detector.TryPlaySensorSound(result);
    }

    private void HighlightSelectedSensor(int result)
    {
        if (sensors == null) return;
        sensors.HighlightSensorByCode(result);
    }

    public void RegisterSensors(Sensors targetSensors)
    {
        sensors = targetSensors;
    }
}
