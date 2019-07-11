using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Employee")
        {
            other.SendMessage("StopPickingMe", gameObject.transform.parent.gameObject);
            if(transform.parent.GetComponent<Obstacle>().PossiblePickers == null)
            {
                transform.parent.GetComponent<Obstacle>().PossiblePickers = new List<GameObject>();
            }
            transform.parent.GetComponent<Obstacle>().PossiblePickers.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Obstacle Parent = transform.parent.GetComponent<Obstacle>();
        if (other.gameObject.tag == "Employee")
        {
            Parent.Pickers = Parent.Pickers?.Where(picker => picker.name != other.gameObject.name).ToList();
            Parent.PossiblePickers = Parent.PossiblePickers?.Where(picker => picker.name != other.gameObject.name).ToList();
        }
    }
}
