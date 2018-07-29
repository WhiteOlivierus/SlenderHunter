using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    public int health = 100;
    public GameObject flash;

    private AudioSource aS;
    private Text healthUI;

    private void Start()
    {
        aS = GetComponent<AudioSource>();
        healthUI = GameObject.Find("Health").GetComponent<Text>();
    }

    void Update()
    {
        //Mouse press
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
            aS.Play();
            StartCoroutine("ShowFlash");
        }

        healthUI.text = health.ToString();
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
                hitObject.GetComponentInParent<SlenderBehaviour>().ResetSlender(true);
            }
        }
    }

    IEnumerator ShowFlash()
    {
        flash.SetActive(true);
        yield return new WaitForSeconds(.1f);
        flash.SetActive(false);
        StopCoroutine("ShowFlash");
    }
}
