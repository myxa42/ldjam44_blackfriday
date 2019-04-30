
using UnityEngine;

public sealed class Weapon : MonoBehaviour
{
    public enum Special
    {
        None,
        // Victim loses life for some time
        Poison,
        // Victim attacks ally for the next move
        Confuse,
        // Victim skips next move
        Stun,
        // Victim causes less damage for some time
        Scare,
        // Victim causes more damage for some time
        Infuriate,
    }

    public enum Visual
    {
        None,
        BaseballBat,
        Knife,
        Shovel,
        Axe,
        Flashlight,
        Scissors,
        Pencil,
        Hammer,
        Wrench,
        PipeWrench,
        Saw,
        Screwdriver,
        Plunger,
        HockeyStick,
        Crowbar,
        Guitar,
        WateringCan,
        Fork,
        Banana,
        FryingPan,
        Ladle,
        Plant,
        CandyCane,
        Fish,
        Racquet,
        Pistol,
        Drill,
        Shotgun,
    }

    public Transform ShotgunParticlesLocation;

    [Header("Weapons")]
    public GameObject BaseballBat;
    public GameObject Knife;
    public GameObject Shovel;
    public GameObject Axe;
    public GameObject Flashlight;
    public GameObject Scissors;
    public GameObject Pencil;
    public GameObject Hammer;
    public GameObject Wrench;
    public GameObject PipeWrench;
    public GameObject Saw;
    public GameObject Screwdriver;
    public GameObject Plunger;
    public GameObject HockeyStick;
    public GameObject Crowbar;
    public GameObject Guitar;
    public GameObject WateringCan;
    public GameObject Fork;
    public GameObject Banana;
    public GameObject FryingPan;
    public GameObject Ladle;
    public GameObject Plant;
    public GameObject CandyCane;
    public GameObject Fish;
    public GameObject Racquet;
    public GameObject Pistol;
    public GameObject Drill;
    public GameObject Shotgun;

    [Header("Effects")]
    public ParticleSystem DrillShoot;
    public ParticleSystem PistolShoot;
    public ParticleSystem ShotgunShoot;

    [Header("Projectile Prefabs")]
    public ProjectileShooter DrillProjectileShooter;
    public ProjectileShooter PistolProjectileShooter;
    public ProjectileShooter ShotgunProjectileShooter;

    void OnDestroy()
    {
        if (ShotgunShoot != null)
            Destroy(ShotgunShoot);
    }

    public void SetVisual(Visual visual)
    {
        BaseballBat.SetActive(visual == Visual.BaseballBat);
        Knife.SetActive(visual == Visual.Knife);
        Shovel.SetActive(visual == Visual.Shovel);
        Axe.SetActive(visual == Visual.Axe);
        Flashlight.SetActive(visual == Visual.Flashlight);
        Scissors.SetActive(visual == Visual.Scissors);
        Pencil.SetActive(visual == Visual.Pencil);
        Hammer.SetActive(visual == Visual.Hammer);
        Wrench.SetActive(visual == Visual.Wrench);
        PipeWrench.SetActive(visual == Visual.PipeWrench);
        Saw.SetActive(visual == Visual.Saw);
        Screwdriver.SetActive(visual == Visual.Screwdriver);
        Plunger.SetActive(visual == Visual.Plunger);
        HockeyStick.SetActive(visual == Visual.HockeyStick);
        Crowbar.SetActive(visual == Visual.Crowbar);
        Guitar.SetActive(visual == Visual.Guitar);
        WateringCan.SetActive(visual == Visual.WateringCan);
        Fork.SetActive(visual == Visual.Fork);
        Banana.SetActive(visual == Visual.Banana);
        FryingPan.SetActive(visual == Visual.FryingPan);
        Ladle.SetActive(visual == Visual.Ladle);
        Plant.SetActive(visual == Visual.Plant);
        CandyCane.SetActive(visual == Visual.CandyCane);
        Fish.SetActive(visual == Visual.Fish);
        Racquet.SetActive(visual == Visual.Racquet);
        Pistol.SetActive(visual == Visual.Pistol);
        Drill.SetActive(visual == Visual.Drill);
        Shotgun.SetActive(visual == Visual.Shotgun);
    }

    public void ShootProjectile(Vector3 target, float time)
    {
        if (Drill.activeSelf) {
            DrillShoot.gameObject.SetActive(true);
            DrillShoot.Play();
            DrillProjectileShooter.ShootProjectile(target, time);
        } else if (Pistol.activeSelf) {
            PistolShoot.gameObject.SetActive(true);
            PistolShoot.Play();
            PistolProjectileShooter.ShootProjectile(target, time);
        } else if (Shotgun.activeSelf) {
            ShotgunShoot.gameObject.SetActive(true);
            ShotgunShoot.Play();

            var t = ShotgunShoot.transform;
            var tt = ShotgunParticlesLocation.transform;
            t.SetParent(null);
            t.position = tt.position;
            t.rotation = tt.rotation;

            ShotgunProjectileShooter.ShootProjectile(target, time);
        }
    }

    public static bool IsRanged(Visual visual)
    {
        switch (visual) {
            case Visual.None:
            case Visual.BaseballBat:
            case Visual.Knife:
            case Visual.Shovel:
            case Visual.Axe:
            case Visual.Flashlight:
            case Visual.Scissors:
            case Visual.Pencil:
            case Visual.Hammer:
            case Visual.Wrench:
            case Visual.PipeWrench:
            case Visual.Saw:
            case Visual.Screwdriver:
            case Visual.Plunger:
            case Visual.HockeyStick:
            case Visual.Crowbar:
            case Visual.Guitar:
            case Visual.WateringCan:
            case Visual.Fork:
            case Visual.Banana:
            case Visual.FryingPan:
            case Visual.Ladle:
            case Visual.Plant:
            case Visual.CandyCane:
            case Visual.Fish:
            case Visual.Racquet:
                return false;

            case Visual.Pistol:
            case Visual.Drill:
            case Visual.Shotgun:
                return true;
        }

        return false;
    }
}
