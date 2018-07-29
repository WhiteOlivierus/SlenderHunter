using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlenderBehaviour : MonoBehaviour
{
    public int damage = 5;
    public float speed = 5.0f;
    public float respawnTime = 10f;
    public float maxDistance = 90f;
    public AudioClip scream;

    private AudioClip basicScarySound;
    private GameObject player;
    private int health = 100;
    private int stage = 0;
    private MeshRenderer mr;
    private AudioSource aS;

    void Start()
    {
        //The kickstart apperence for the slender
        player = GameObject.Find("FPSController");
        mr = GetComponentInChildren<MeshRenderer>();
        aS = GetComponentInChildren<AudioSource>();
        basicScarySound = aS.clip;
        StartCoroutine("WaitForRespawn", respawnTime);

    }

    private void Update()
    {
        //Checks if the player can see slender
        SlenderInView();
    }

    private void SlenderInView()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

        if (onScreen && mr.enabled)
        {
            //disapears in couple of seconds
            print("I see you");
        }
    }
    private void PlaceSlender()
    {
        ResetTrees();

        //all the trees in range
        List<Collider> stageSelected = new List<Collider>(Physics.OverlapSphere(player.transform.position, (maxDistance - (30 * stage)) / 2, 1 << 8));

        //Remove the trees from stage 2 range
        for (int i = 0; i < stageSelected.Count; i++)
        {
            if (stage != 2)
            {
                List<Collider> stageToRemove = new List<Collider>(Physics.OverlapSphere(player.transform.position, (maxDistance - (30 * (stage + 1))) / 2, 1 << 8));

                for (int j = 0; j < stageToRemove.Count; j++)
                {
                    if (stageSelected[i].transform == stageToRemove[j].transform)
                    {
                        stageSelected.Remove(stageToRemove[j]);
                    }
                }
            }
            //show the current stage trees selected -- for debugging only
            stageSelected[i].GetComponent<Renderer>().material.color = Color.green;
        }

        //Selecet the tree that slender will appear at
        Transform selectedTree = stageSelected[Random.Range(0, stageSelected.Count)].transform;
        selectedTree.GetComponent<Renderer>().material.color = Color.yellow; // -- for debugging only

        //Change location to slender
        gameObject.transform.position = new Vector3(selectedTree.position.x, gameObject.transform.position.y, selectedTree.position.z);
        gameObject.transform.localPosition -= gameObject.transform.forward;

        //Rotate the slender towards me
        Vector3 direction = player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 5 * Time.deltaTime);
    }

    public void ResetSlender(bool hit)
    {
        //Reset the slender attack loop
        stage = 0;
        StopCoroutine("LifeSpan");
        StartCoroutine("WaitForRespawn", respawnTime);

         //check what kind of damage the should be
        if (hit)
        {
            health -= 10;
            print("slender was hit");
        }
        else
        {
            player.GetComponentInChildren<PlayerBehaviour>().health -= 10;
            aS.clip = scream;
            aS.loop = false;
            aS.Play();
            print("slender hit player");
        }  
    }

    private void ResetTrees()
    {
        //Reset trees -- for debugging only
        List<Collider> resetTrees = new List<Collider>(Physics.OverlapSphere(player.transform.position, 200f, 1 << 8));

        for (int l = 0; l < resetTrees.Count; l++)
        {
            resetTrees[l].GetComponent<Renderer>().material.color = new Color(0.26f, 0.17f, 0.09f);
        }
    }

    IEnumerator WaitForRespawn(float respawnTime)
    {
        //hide the slender for a reset attack loop
        print("Killed"); // -- for debugging only
        mr.enabled = false;  
        yield return new WaitForSecondsRealtime(respawnTime);

        //Find the location that the slender has to stand
        PlaceSlender();

        //show the slender
        StartCoroutine("LifeSpan", speed);
    }

    IEnumerator LifeSpan(float lifeTime)
    {
        aS.loop = true;
        aS.clip = basicScarySound;

        //toggle the visibilty of the slender
        if (!mr.enabled)
        {
            print("Spawned"); // -- for debugging only
            mr.enabled = true;
            aS.Play();
        }
        else
        {
            print("Hidden"); // -- for debugging only
            mr.enabled = false;
            aS.Stop();

            //print(stage);
            if (stage < 2)
            {
                stage++;
            }
            else
            {
                //Do damage to player
                //reset the slender for next attack round
                ResetSlender(false);
            }
        }

        //wait seconds before reappering or hidding
        yield return new WaitForSecondsRealtime(lifeTime);

        //Find the location that the slender has to stand
        PlaceSlender();

        //show the slender
        StartCoroutine("LifeSpan", speed);  
    }
}
