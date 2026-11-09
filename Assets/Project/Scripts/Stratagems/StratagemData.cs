using UnityEngine;

[CreateAssetMenu(fileName = "NewStratagem", menuName = "Game/Stratagem Data")]
public class StratagemData : ScriptableObject
{
    public string stratagemName = "Resupply";
    public Direction[] code;
    public float cooldown = 45f;
    public int maxUses = 0;          // 0이면 횟수 제한 없음 (쿨타임으로만 제한)
    public float callDelay = 3f;     // 비컨 착지 후 지원이 떨어지기까지의 시간
    public Color beaconColor = Color.blue;
}