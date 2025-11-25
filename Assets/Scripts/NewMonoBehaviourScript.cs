using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private bool rotateClockwise = false;
    public GameObject gameobject;
    public GameObject gameobject2;
    
    public void Start()
    {
        float angle = rotateClockwise ? -50f : 50f;

        transform.Rotate(0, 0, angle);
        rotateClockwise = !rotateClockwise;

    }

    public void recrut()
    {
        if (gameobject.activeSelf)
        {
            gameobject.SetActive(false);
            gameobject2.SetActive(true);
        } else
        {
            gameobject.SetActive(true);
            gameobject2.SetActive(false);
        }
    }
   
}
