using GameAnalyticsSDK;
using UnityEngine;

public class InitAnalytics : MonoBehaviour
{
    void Awake()
    {
        GameAnalytics.Initialize();
    }
}
