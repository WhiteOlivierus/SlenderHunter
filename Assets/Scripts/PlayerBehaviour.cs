using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    public GameObject flash;
    public int health = 100;

    private AudioSource aS;
    private Text healthUI;
    private GameManager gm;

    private void Start()
    {
        gm = GameObject.Find("FPSController").GetComponent<GameManager>();
        aS = GetComponent<AudioSource>();
        healthUI = GameObject.Find("Health").GetComponent<Text>();
    }

    void Update()
    {
        //Mouse press
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        healthUI.text = health.ToString();

        gm.DeadCheck(health);
    }

    private void Shoot()
    {
        StartCoroutine("ShowFlash");
        aS.Play();

        RaycastHit rayHit;

        //shoot a raycast
        if (Physics.Raycast(transform.position, transform.forward, out rayHit))
        {
            GameObject hitObject = rayHit.collider.gameObject;
            // if hit a slender return the hit and remove health
            if (hitObject.name == "Slender")
            {
                hitObject.GetComponentInParent<SlenderBehaviour>().ResetSlender(0);
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
