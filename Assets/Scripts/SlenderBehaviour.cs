using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlenderBehaviour : MonoBehaviour
{
    public int health = 100;
    public float speed = 5.0f;
    public int damage = 5;
    public float respawnTime = 10f;
    public int amountOfStages = 3;
    public float maxDistance = 90f;
    public AudioClip scream;
    public int layer = 8;

    private AudioClip basicScarySound;
    private GameObject player;
    private int stage = 0;
    private MeshRenderer mr;
    private AudioSource audioS;
    private Text healthUI;
    private GameManager gm;

    void Start()
    {
        //init variables
        gm = GameObject.Find("FPSController").GetComponent<GameManager>();
        healthUI = GameObject.Find("HealthSlender").GetComponent<Text>();
        player = GameObject.Find("FPSController");
        mr = GetComponentInChildren<MeshRenderer>();
        audioS = GetComponentInChildren<AudioSource>();
        basicScarySound = audioS.clip;

        StartCoroutine("WaitForRespawn", respawnTime);
    }

    private void Update()
    {
        //Checks if the player can see slender
        SlenderInView();
        print(stage);
        transform.rotation = RotateSlenderTowards(transform, player.transform);
    }

    private Quaternion RotateSlenderTowards(Transform orgin, Transform target)
    {
        //Rotate the slender towards me
        Vector3 direction = target.position - orgin.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        return Quaternion.Lerp(transform.rotation, rotation, 5 * Time.deltaTime);
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
#if UNITY_EDITOR
        ResetTrees();
#endif

        Vector3 selectedTree = GetTree(player.transform, maxDistance,amountOfStages, stage, layer);

        //Change location to slender
        gameObject.transform.position = new Vector3(selectedTree.x, gameObject.transform.position.y, selectedTree.z);
        gameObject.transform.localPosition -= gameObject.transform.forward;
    }

    private Vector3 GetTree(Transform origin, float distance,int stages ,int currentStage = 0, int layer = 0)
    {
        //all the trees in range
        List<Collider> stageSelected = new List<Collider>(Physics.OverlapSphere(origin.position, (distance - ((distance / stages) * currentStage)) / 2, 1 << layer));

        //Remove the trees from stage 2 range
        for (int i = 0; i < stageSelected.Count; i++)
        {
            if (currentStage != 2)
            {
                List<Collider> stageToRemove = new List<Collider>(Physics.OverlapSphere(player.transform.position, (distance - ((distance / stages) * (currentStage + 1))) / 2, 1 << layer));

                for (int j = 0; j < stageToRemove.Count; j++)
                {
                    if (stageSelected[i].transform == stageToRemove[j].transform)
                    {
                        stageSelected.Remove(stageToRemove[j]);
                    }
                }
            }
#if UNITY_EDITOR
            //show the current stage trees selected -- for debugging only
            stageSelected[i].GetComponent<Renderer>().material.color = Color.green;
#endif
        }

        //Selecet the tree that slender will appear at
        Transform selectedTree = stageSelected[Random.Range(0, stageSelected.Count)].transform;

#if UNITY_EDITOR
        selectedTree.GetComponent<Renderer>().material.color = Color.yellow; // -- for debugging only
#endif
        return selectedTree.position;
    }

    public void ResetSlender(int hit)
    {
         //check what kind of damage the should be
        if (hit == 0)
        {
            stage = 0;
            health -= damage;
            print("slender waudioS hit");
            healthUI.text = health.ToString();
            gm.DeadCheck(health);
        }
        else if(hit == 1)
        {
            audioS.clip = scream;
            audioS.loop = false;
            audioS.Play();
            stage = 0;
            player.GetComponentInChildren<PlayerBehaviour>().health -= damage;
            print("slender hit player");
        }

        //Reset the slender attack loop     
        StopAllCoroutines();
        StartCoroutine("WaitForRespawn", respawnTime);
    }

#if UNITY_EDITOR
    private void ResetTrees()
    {
        //Reset trees -- for debugging only
        List<Collider> resetTrees = new List<Collider>(Physics.OverlapSphere(player.transform.position, maxDistance*amountOfStages, 1 << 8));

        for (int l = 0; l < resetTrees.Count; l++)
        {
            resetTrees[l].GetComponent<Renderer>().material.color = new Color(0.26f, 0.17f, 0.09f);
        }
    }
#endif

    IEnumerator WaitForRespawn(float respawnTime)
    {
        //hide the slender for a reset attack loop
        print("Hidden"); // -- for debugging only
        mr.enabled = false;
        if (audioS.clip != scream)
            audioS.Stop();

        //print(stage);
        if (stage < 2)
        {
            stage++;
        }
        else
        {
            //Do damage to player
            //reset the slender for next attack round
            ResetSlender(1);
        }

        yield return new WaitForSecondsRealtime(respawnTime);

        //Find the location that the slender haudioS to stand
        PlaceSlender();

        //show the slender
        StartCoroutine("LifeSpan", speed);
    }

    IEnumerator LifeSpan(float lifeTime)
    {
        audioS.loop = true;
        audioS.clip = basicScarySound;

        print("Spawned"); // -- for debugging only
        mr.enabled = true;
        audioS.Play();

        //wait seconds before reappering or hidding
        yield return new WaitForSecondsRealtime(lifeTime);

        ResetSlender(-1);
    }
}
