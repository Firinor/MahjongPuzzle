using UnityEngine;

public class CheatsDestroer : MonoBehaviour
{
    void Awake()
    {
#if Cheats
        return;
#else
        Destroy(gameObject);
#endif
    }
}
