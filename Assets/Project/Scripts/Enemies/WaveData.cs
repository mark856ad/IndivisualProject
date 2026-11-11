using UnityEngine;

[System.Serializable]
public class WaveEntry
{
    public Enemy prefab;
    public float weight = 1f;
}

[CreateAssetMenu(fileName = "NewWave", menuName = "Game/Wave Data")]
public class WaveData : ScriptableObject
{
    public WaveEntry[] entries;

    [Header("압박 0일 때 (시작)")]
    public float startInterval = 4f;
    public int startMaxAlive = 3;

    [Header("압박 1일 때 (최대)")]
    public float minInterval = 1.5f;
    public int endMaxAlive = 10;

    [Header("시간 기준 압박")]
    public float timeToMaxPressure = 120f;
}
