using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int initialPoolSize = 20;
    [SerializeField] private int maxPoolSize = 50;
    [SerializeField] private bool autoExpand = true;
    
    private Queue<Bullet> availableBullets = new Queue<Bullet>();
    private HashSet<Bullet> activeBullets = new HashSet<Bullet>();
    private Transform poolContainer;
    
    void Awake()
    {
        poolContainer = new GameObject("BulletPoolContainer").transform;
        poolContainer.SetParent(transform);
        
        InitializePool();
    }
    
    void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateBullet();
        }
        
        Debug.Log($"<color=cyan>[BULLET POOL] Initialized with {initialPoolSize} bullets</color>");
    }
    
    Bullet CreateBullet()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, poolContainer);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        
        if (bullet == null)
        {
            Debug.LogError("[BULLET POOL] Bullet prefab is missing Bullet component!");
            Destroy(bulletObj);
            return null;
        }
        
        bulletObj.SetActive(false);
        availableBullets.Enqueue(bullet);
        
        return bullet;
    }
    
    public Bullet GetBullet(BulletData data, Vector2 position, Vector2 direction, GameObject owner)
    {
        Bullet bullet = null;
        
        if (availableBullets.Count > 0)
        {
            bullet = availableBullets.Dequeue();
        }
        else if (autoExpand && activeBullets.Count + availableBullets.Count < maxPoolSize)
        {
            bullet = CreateBullet();
            Debug.LogWarning($"<color=yellow>[BULLET POOL] Expanding pool. Active: {activeBullets.Count}</color>");
        }
        else
        {
            Debug.LogWarning($"<color=red>[BULLET POOL] Pool exhausted! Active: {activeBullets.Count}/{maxPoolSize}</color>");
            return null;
        }
        
        if (bullet != null)
        {
            bullet.transform.position = position;
            bullet.gameObject.SetActive(true);
            bullet.Initialize(data, direction, owner, this);
            activeBullets.Add(bullet);
        }
        
        return bullet;
    }
    
    public void ReturnBullet(Bullet bullet)
    {
        if (bullet == null || !activeBullets.Contains(bullet))
            return;
        
        activeBullets.Remove(bullet);
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(poolContainer);
        availableBullets.Enqueue(bullet);
    }
    
    public void ClearAllBullets()
    {
        foreach (Bullet bullet in activeBullets)
        {
            if (bullet != null)
            {
                bullet.gameObject.SetActive(false);
                bullet.transform.SetParent(poolContainer);
                availableBullets.Enqueue(bullet);
            }
        }
        
        activeBullets.Clear();
        Debug.Log("<color=cyan>[BULLET POOL] All bullets cleared</color>");
    }
    
    public int GetActiveCount() => activeBullets.Count;
    public int GetAvailableCount() => availableBullets.Count;
}
