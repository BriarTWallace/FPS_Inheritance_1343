using UnityEngine;

public class Automatic : Gun
{
    public override bool AttemptFire()
    {
        if (!base.AttemptFire())
            return false;

        var b = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, gunBarrelEnd.rotation);
        b.GetComponent<Projectile>().Initialize(1, 200, 2, 1, null); // version without special effect

        anim.SetTrigger("shoot");
        elapsed = 0;
        ammo -= 1;

        return true;
    }
}
