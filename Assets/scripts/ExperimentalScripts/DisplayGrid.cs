using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayGrid : MonoBehaviour
{

    // Start is called before the first frame update
    private Vector3 selectedPoint;
 
    public Vector3 getPointClicked()
    {
        Debug.Log("Returning Coordinates: " + selectedPoint);
        return selectedPoint;
    }

    void OnMouseDown()
    {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
    if (Physics.Raycast(ray, out hit))
    {
        Vector3 pointWorldPos = hit.point;
        selectedPoint = hit.point;
       
        //getPointClicked(pointWorldPos);
         Debug.Log("Clicked at: " + selectedPoint);
        
    }
    }
}
