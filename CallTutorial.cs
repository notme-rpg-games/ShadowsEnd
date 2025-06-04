using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        //wait thus bool city map is definetely set to false
        yield return new WaitForSecondsRealtime(1);
        if(GameManager.Instance.tutorialTips)
        {
            GameManager.Instance.tipIndex = 1;
            GameManager.Instance.Tutorial();
            UnitSelections.Instance.DisableUnitSelections();
        }
    }

}
