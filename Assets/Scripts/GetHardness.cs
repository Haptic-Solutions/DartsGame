using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetHardness : MonoBehaviour
{
    public AnimateFingers animatedFingers;
    public FINGER fingerNumber = FINGER.FINGER_MIDDLE;
    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("Entered object " + collision.gameObject.name);
        Hardness hardnessObj = collision.gameObject.GetComponent<Hardness>();
        if(hardnessObj != null)
        {
            var hardnessVal = hardnessObj.GetHardness();
            animatedFingers.SetHardness(fingerNumber, hardnessVal);
            hardnessObj.SetTouch(fingerNumber, true, animatedFingers.gameObject);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        //Debug.Log("Exited object " + collision.gameObject.name);
        Hardness hardnessObj = collision.gameObject.GetComponent<Hardness>();
        if (hardnessObj != null)
        {
            var hardnessVal = hardnessObj.GetHardness();
            animatedFingers.SetHardness(fingerNumber, 0.15f);
            hardnessObj.SetTouch(fingerNumber, false, null);
        }
    }

}
