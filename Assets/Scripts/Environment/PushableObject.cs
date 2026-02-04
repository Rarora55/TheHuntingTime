using UnityEngine;
using TheHunt.Environment;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PushableObject : MonoBehaviour, IPushable
{
    [Header("Configuration")]
    [SerializeField] private PushableObjectData pushableData;
    
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.9f, 0.1f);
    [SerializeField] private LayerMask groundLayer;
    
    private Rigidbody2D rb2D;
    private bool isPushing;
    private bool isPulling;
    private Player currentPusher;
    private Transform originalParent;
    
    private AudioSource audioSource;
    
    public PushableObjectData PushableData => pushableData;
    public bool CanBePushed => pushableData != null && pushableData.canBePushed && !isPulling;
    public bool CanBePulled => pushableData != null && pushableData.canBePulled && !isPushing;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        
        if (rb2D != null)
        {
            rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb2D.bodyType = RigidbodyType2D.Kinematic;
        }
        
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }
        
        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Ground", "Wall");
        }
    }

    public void OnPushStart()
    {
        isPushing = true;
        
        if (rb2D != null)
        {
            rb2D.bodyType = RigidbodyType2D.Dynamic;
        }
        
        if (audioSource != null && pushableData != null)
        {
            if (pushableData.pushSound != null)
            {
                audioSource.PlayOneShot(pushableData.pushSound);
            }
            
            if (pushableData.pushLoopSound != null)
            {
                audioSource.clip = pushableData.pushLoopSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    public void OnPushEnd()
    {
        isPushing = false;
        
        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
            StartCoroutine(CheckGroundAndSetKinematic());
        }
        
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    
    private IEnumerator CheckGroundAndSetKinematic()
    {
        yield return new WaitForSeconds(0.1f);
        
        bool isGrounded = CheckIfGrounded();
        
        if (isGrounded)
        {
            rb2D.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            
            if (rb2D.linearVelocity.magnitude < 0.1f)
            {
                rb2D.bodyType = RigidbodyType2D.Kinematic;
            }
            else
            {
                StartCoroutine(WaitForLanding());
            }
        }
    }
    
    private IEnumerator WaitForLanding()
    {
        while (!CheckIfGrounded() && rb2D.linearVelocity.magnitude > 0.1f)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(0.2f);
        rb2D.bodyType = RigidbodyType2D.Kinematic;
    }
    
    private bool CheckIfGrounded()
    {
        if (groundCheck == null) return true;
        
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
    }

    public void OnPullStart()
    {
        isPulling = true;
        
        if (audioSource != null && pushableData != null)
        {
            if (pushableData.pullSound != null)
            {
                audioSource.PlayOneShot(pushableData.pullSound);
            }
            
            if (pushableData.pullLoopSound != null)
            {
                audioSource.clip = pushableData.pullLoopSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    public void OnPullEnd()
    {
        isPulling = false;
        
        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
            StartCoroutine(CheckGroundAndSetKinematic());
        }
        
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public Transform GetObjectTransform()
    {
        return transform;
    }

    public Rigidbody2D GetRigidbody()
    {
        return rb2D;
    }

    public void SetCurrentPusher(Player pusher)
    {
        currentPusher = pusher;
    }

    public Player GetCurrentPusher()
    {
        return currentPusher;
    }
    
    public void AttachToPlayer(Transform pullPoint)
    {
        originalParent = transform.parent;
        transform.SetParent(pullPoint);
    }
    
    public void DetachFromPlayer()
    {
        transform.SetParent(originalParent);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}
