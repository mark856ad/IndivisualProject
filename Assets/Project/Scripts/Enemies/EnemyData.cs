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
}