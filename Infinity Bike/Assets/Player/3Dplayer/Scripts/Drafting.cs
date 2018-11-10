using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drafting : MonoBehaviour
{   

    private EnvironementObserver environementObserver;
    [SerializeField]
    public EnvironementObserver.LayerToReact layerToReact;
    private Rigidbody rb;

    private Movement movement;
    public float draftingDrag = -1f;
    GameObject following = null;
    public GameObject followingIndicatorPrefab = null;
    private GameObject followingIndicator = null;

    public float folowingTime;
    public float timeBeforeUpdatingFollowingSpeed = 1;

    float targetSpeedbefore = 0;
    AIDriver followingAiDriver = null;
    

    void Start ()
    {   
        environementObserver = GetComponent<EnvironementObserver>();
        movement = GetComponent<Movement>();
        rb = this.GetComponent<Rigidbody>();
        followingIndicator = Instantiate<GameObject>(followingIndicatorPrefab);
        followingIndicator.SetActive(false);
    }   



    private void Update()
    {   
        environementObserver.TallyUpCommingObstacles();
        if (following != null)
        {
            folowingTime += Time.deltaTime;
        }
        else
        {   
            folowingTime = 0;
        }   

        if (folowingTime > timeBeforeUpdatingFollowingSpeed)
        {   
            
            followingAiDriver.aiSettings.targetSqrSpeed = Vector3.Dot(rb.velocity , rb.velocity);
        }       
    }   
    
    
    
    void FixedUpdate ()
    {
        GameObject newFollowing = null;
        if (CheckIfFollowingDriver(out newFollowing))
        {   
            if (following != newFollowing && newFollowing != null)
            {
                if(followingAiDriver!=null)
                followingAiDriver.aiSettings.targetSqrSpeed = targetSpeedbefore;

                following = newFollowing;
                followingAiDriver = following.GetComponent<AIDriver>();
                targetSpeedbefore = followingAiDriver.aiSettings.targetSqrSpeed;

                SetDraftingMarker();
            }   
            
            ApplyDrag();
            
        }   
        else
        {   
            following = null;
            followingIndicator.SetActive(false);
        }   



    }

    private void SetDraftingMarker()
    {   
        followingIndicator.transform.parent = following.transform;
        followingIndicator.SetActive(true);
        followingIndicator.transform.localPosition = new Vector3(0, 0.5f, 0);
    }   

    protected bool CheckIfFollowingDriver(out GameObject obj)
    {
        GameObject newFollowing = null;
        float closestDistance = float.MaxValue;
        bool hitFound = false;
        if (environementObserver.hit != null)
        {
            foreach (RaycastHit item in environementObserver.hit)
            {
                if (((1 << item.transform.gameObject.layer) & layerToReact.npcLayer.value) != 0)
                {
                    hitFound = true;
                    float distance = Vector3.Distance(transform.position, item.transform.position);

                    if (distance < closestDistance)
                    { distance = closestDistance; }
                    newFollowing = item.collider.gameObject;



                }
            }
        }

        if (!hitFound)
        {
            obj = null;
        }
        else
        {

            obj = newFollowing;
        }   

        return hitFound;
    }

    void ApplyDrag()
    {
        if (following != null)
        {
            float distance = (following.transform.position - transform.position).sqrMagnitude;

            if (distance > 0.01f)
            { movement.ApplyVelocityDrag(draftingDrag * (1 - distance / (environementObserver.distanceToCheck* environementObserver.distanceToCheck))); }
            else
            { movement.ApplyVelocityDrag(draftingDrag); }
        }
    }


}
