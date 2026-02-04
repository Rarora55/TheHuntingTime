using UnityEngine;
using TheHunt.Environment;

public class PlayerPushPullController : MonoBehaviour
{
    private Player player;
    private PlayerData playerData;
    
    private IPushable currentPushable;
    private bool isPushing;
    private bool isPulling;

    public IPushable CurrentPushable => currentPushable;
    public bool IsPushing => isPushing;
    public bool IsPulling => isPulling;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void Initialize(PlayerData data)
    {
        playerData = data;
    }

    public IPushable DetectNearbyPushable()
    {
        if (playerData == null)
            return null;

        Vector2 detectionPoint = (Vector2)player.transform.position + Vector2.right * player.FacingRight * 0.5f;
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            detectionPoint, 
            playerData.pushPullDetectionRadius, 
            playerData.pushableObjectLayer
        );

        IPushable closestPushable = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            IPushable pushable = hit.GetComponent<IPushable>();
            
            if (pushable == null)
                continue;

            Vector2 directionToObject = hit.transform.position - player.transform.position;
            float dotProduct = directionToObject.normalized.x * player.FacingRight;
            
            if (dotProduct > 0.5f)
            {
                float distance = Vector2.Distance(player.transform.position, hit.transform.position);
                
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPushable = pushable;
                }
            }
        }

        return closestPushable;
    }

    public bool CanInteractWith(IPushable pushable, bool isPush)
    {
        if (pushable == null)
            return false;

        if (isPush)
            return pushable.CanBePushed;
        else
            return pushable.CanBePulled;
    }

    public float CalculateSpeedPenalty(IPushable pushable)
    {
        if (pushable == null || pushable.PushableData == null || playerData == null)
            return playerData.movementVelocity;

        float baseSpeed = playerData.movementVelocity;
        float objectWeight = pushable.PushableData.baseWeight;
        
        float speedMultiplier = playerData.basePushPullSpeedMultiplier;
        
        float extraWeight = Mathf.Max(0, objectWeight - playerData.minimumWeight);
        float weightPenalty = extraWeight * playerData.weightPenaltyPerUnit;
        
        float finalSpeed = baseSpeed * speedMultiplier - weightPenalty;
        
        finalSpeed = Mathf.Max(finalSpeed, 3.0f);
        
        return finalSpeed;
    }

    public void StartPushing(IPushable pushable)
    {
        if (pushable == null)
            return;

        currentPushable = pushable;
        isPushing = true;
        isPulling = false;
        
        pushable.OnPushStart();
        
        if (pushable is PushableObject pushableObj)
        {
            pushableObj.SetCurrentPusher(player);
        }
    }

    public void StopPushing()
    {
        if (currentPushable != null)
        {
            currentPushable.OnPushEnd();
            
            if (currentPushable is PushableObject pushableObj)
            {
                pushableObj.SetCurrentPusher(null);
            }
        }

        currentPushable = null;
        isPushing = false;
    }

    public void StartPulling(IPushable pushable)
    {
        if (pushable == null)
            return;

        currentPushable = pushable;
        isPulling = true;
        isPushing = false;
        
        pushable.OnPullStart();
        
        if (pushable is PushableObject pushableObj)
        {
            pushableObj.SetCurrentPusher(player);
        }
    }

    public void StopPulling()
    {
        if (currentPushable != null)
        {
            currentPushable.OnPullEnd();
            
            if (currentPushable is PushableObject pushableObj)
            {
                pushableObj.SetCurrentPusher(null);
            }
        }

        currentPushable = null;
        isPulling = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (playerData == null || player == null)
            return;

        Gizmos.color = Color.yellow;
        Vector2 detectionPoint = (Vector2)transform.position + Vector2.right * player.FacingRight * 0.5f;
        Gizmos.DrawWireSphere(detectionPoint, playerData.pushPullDetectionRadius);
    }
}
