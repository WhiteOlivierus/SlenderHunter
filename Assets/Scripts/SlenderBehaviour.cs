using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlenderBehaviour : MonoBehaviour
{
    public GameObject slenderObject;
    public  int damage = 5;
    public float speed = 5.0f;
    public float minRespawnTime = 10f;
    public float respawnTimeBias = 10f;
    public float maxDistance = 90f;

    private int health = 100;
    private bool hit = false;
    private int stage = 0;
    
	void Start ()
    {
        //The kickstart apperence for the slender
        float respawnTime = Random.Range(minRespawnTime, minRespawnTime + respawnTimeBias);
        StartCoroutine("WaitForRespawn", respawnTime);
	}

    void Update()
    {
        //Mouse press
        if (Input.GetMouseButton(0))
        {
            //shoot a ray returns if hit slender
            hit = Shoot();

            if (hit)
            {
                //Reset the slender attack loop
                stage = 0;
                StopCoroutine("LifeSpan");
                float respawnTime = Random.Range(minRespawnTime, minRespawnTime + respawnTimeBias);
                StartCoroutine("WaitForRespawn", respawnTime);
            }
        }

        //Rotate the slender towards me
        slenderObject.transform.LookAt(transform.position);
    }

    private void Respawn()
    {
        //Find the location that the slender has to stand
        PlaceSlender();

        //show the slender
        StartCoroutine("LifeSpan", speed);
    }

    private void PlaceSlender()
    {
        //Reset trees -- for debugging only
        List<Collider> resetTrees = new List<Collider>(Physics.OverlapSphere(transform.position, 200f, 1 << 8));
        for (int l = 0; l < resetTrees.Count; l++)
        {
            resetTrees[l].GetComponent<Renderer>().material.color = new Color(0.26f, 0.17f,0.09f);
        }

        //all the trees in range
        List<Collider> stageOneSelected = new List<Collider>(Physics.OverlapSphere(transform.position, maxDistance - (20 * stage), 1 << 8));
        List<Collider> stageTwoSelected = new List<Collider>(Physics.OverlapSphere(transform.position, maxDistance - 20, 1 << 8));

        //Remove the trees from stage 2 range
        for (int i = 0; i < stageOneSelected.Count; i++)
        {
            for (int j = 0; j < stageTwoSelected.Count; j++)
            {
                if(stageOneSelected[i].transform == stageTwoSelected[j].transform)
                {
                    stageOneSelected.Remove(stageTwoSelected[j]);
                }
            }

            //show the current stage trees selected -- for debugging only
            stageOneSelected[i].GetComponent<Renderer>().material.color = Color.green;
        }

        //Selecet the tree that slender will appear at
        Transform selectedTree = stageOneSelected[Random.Range(0, stageOneSelected.Count)].transform;
        selectedTree.GetComponent<Renderer>().material.color = Color.yellow; // -- for debugging only

        //Change location to slender
        slenderObject.transform.position = selectedTree.position;
        slenderObject.transform.localPosition -= slenderObject.transform.forward;
    }

    IEnumerator WaitForRespawn(float respawnTime)
    {
        //hide the slender for a reset attack loop
        print("Killed"); // -- for debugging only
        slenderObject.SetActive(false);
        yield return new WaitForSecondsRealtime(respawnTime);
        Respawn();
    }

    IEnumerator LifeSpan(float lifeTime)
    {
        //toggle the visibilty of the slender
        if (!slenderObject.activeSelf)
        {
            print("Spawned"); // -- for debugging only
            slenderObject.SetActive(true);
        }
        else
        {
            print("Hidden"); // -- for debugging only
            slenderObject.SetActive(false);
        }

        //wait seconds before reappering or hidding
        yield return new WaitForSecondsRealtime(lifeTime);

        //if the slender is not hit it moves closer
        if(!hit)
        {
            print(stage);
            stage++;
            Respawn();
        }
    }

    private bool Shoot()
    {
        RaycastHit rayHit;

        //shoot a raycast
        if(Physics.Raycast(transform.position, transform.forward, out rayHit))
        {

            // if hit a slender return the hit and remove health
            if(rayHit.collider.gameObject.name == "Slender")
            {
                health -= 5;
                return true;
            }
        }

        return false;
    }
}
