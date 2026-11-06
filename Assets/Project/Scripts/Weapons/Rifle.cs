using UnityEngine;

public class Rifle : Weapon
{
    protected override void Fire()
    {
        Bullet bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
        bullet.Damage = data.damage;
    }
}