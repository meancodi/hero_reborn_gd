using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool instance;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 20;

    private GameObject[] pool;
    private int index = 0;

    private void Awake()
    {
        instance = this;

        pool = new GameObject[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Instantiate(bulletPrefab);
            pool[i].SetActive(false);
        }
    }

    public GameObject GetBullet()
    {
        // cycle index
        index++;
        if (index >= pool.Length) index = 0;

        GameObject bullet = pool[index];
        bullet.SetActive(false);  // ensure clean reset
        return bullet;
    }
}
