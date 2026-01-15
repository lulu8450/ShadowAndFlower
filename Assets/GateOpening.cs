using System.Collections.Generic;
using UnityEngine;

public class GateOpening : MonoBehaviour
{
    public bool isOpen;
    [SerializeField] private Animator gateAnimator;
    [SerializeField] private List<leveractivator> levers;

    // Update is called once per frame
    void Update()
    {
        if (AllLeversActivated())
        {
            gateAnimator.Play("JailWall_On");
            isOpen = true;
        }
        else
        {
            gateAnimator.Play("JailWall_Idle");
        }
    }
    private bool AllLeversActivated()
    {
        foreach (var lever in levers)
        {
            if (!lever.isActivate)
            {
                return false;
            }
        }
        return true;
    }
}
