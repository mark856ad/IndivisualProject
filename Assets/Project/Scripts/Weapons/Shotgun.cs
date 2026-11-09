using UnityEngine;

public class Shotgun : Weapon   
{
    protected override void Fire()
    {
        int count = data.pelletCount;
        float spread = data.spreadAngle;
        float startAngle = -spread / 2f;
        float step = count > 1 ? spread / (count - 1) : 0f;

        for (int i = 0; i < count; i++)
        {
            float angle = count > 1 ? startAngle + step * i : 0f;
            Quaternion rotation = muzzle.rotation * Quaternion.Euler(0f, angle, 0f);

            SpawnBullet(rotation);
        }
    }
}
