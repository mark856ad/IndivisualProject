using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName = "Grunt";
    public float maxHealth = 30f;
    public float moveSpeed = 3.5f;
    public float attackRange = 1.5f;
    public float detectRange = 15f;
    public float loseRange = 20f;
    public float wanderRadius = 5f;
    public float wanderWaitTime = 2f;
    public float attackDamage = 10f;
    public float attackInterval = 1f;
    public bool keepDistance = false;     // 켜면 원거리형 (거리를 유지하며 물러남)
    public float preferredRange = 8f;     // 이보다 가까워지면 물러남
    public float retreatStep = 4f;        // 물러날 때 한 번에 정하는 거리
    public float viewAngle = 120f;   // 시야각(도). 정면 기준 좌우로 절반씩
    public float loseSightTime = 3f;   // 플레이어를 놓친 뒤 추격을 포기하기까지의 시간(초)
}