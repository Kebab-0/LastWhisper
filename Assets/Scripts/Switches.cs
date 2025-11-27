using UnityEngine;
using UnityEngine.UI;

public class Switches : MonoBehaviour
{

    public Animator animator;

    private bool isOn = false;

   

    public void ToggleSwitch()
    {
        isOn = !isOn;
        animator.SetBool("isOn", isOn);
        
    }

    
}