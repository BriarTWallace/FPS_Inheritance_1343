using UnityEngine;

public class Automatic : Gun
{
    [SerializeField] GameObject particleEffect;
    public override bool AttemptFire()
    {
        if (!base.AttemptFire())
            return false;

        var b = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, gunBarrelEnd.rotation);
        b.GetComponent<Projectile>().Initialize(1, 200, 2, 1, HitUpward); // version without special effect

        anim.SetTrigger("shoot");
        elapsed = 0;
        ammo -= 1;

        return true;
    }

    // example function, make hit enemy fly upward
    void HitUpward(HitData data)
    {
        Vector3 impactLocation = data.location;

        Instantiate(particleEffect, impactLocation, Quaternion.identity);

        var colliders = Physics.OverlapSphere(impactLocation, 10);
        foreach (var c in colliders)
        {
            if (c.GetComponent<Rigidbody>())
            {
                c.GetComponent<Rigidbody>().AddForce(Vector3.up * 20, ForceMode.Impulse);
            }
        }
    }
}
