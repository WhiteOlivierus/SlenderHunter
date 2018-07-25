using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public int health = 100;

    void Update()
    {
        //Mouse press
        if (Input.GetMouseButton(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        RaycastHit rayHit;

        //shoot a raycast
        if (Physics.Raycast(transform.position, transform.forward, out rayHit))
        {
            GameObject hitObject = rayHit.collider.gameObject;
            // if hit a slender return the hit and remove health
            if (hitObject.name == "Slender")
            {
                hitObject.GetComponent<SlenderBehaviour>().ResetSlender(true);
            }
        }
    }
}
