using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("기본")]
    public string weaponName = "Rifle";

    [Header("공격")]
    public float damage = 10f;
    public float fireInterval = 0.1f;   // 한 발 쏘고 다음 발까지 대기 시간(초)
    public float range = 30f;
    
    [Header("산탄")]
    public int pelletCount = 1;
    
    [Header("탄약")]
    public int magazineSize = 30;
    public int reserveMagazines = 4;
    public bool infiniteReserve = false;   // 추가: 켜면 예비 탄창이 줄지 않음 (권총용)
    public float reloadTime = 2f;

    [Header("탄 퍼짐")]
    public float spreadAngle = 2f;      // 퍼짐 각도(도)
    
    [Header("사운드")]
    public AudioClip fireSound;
}
