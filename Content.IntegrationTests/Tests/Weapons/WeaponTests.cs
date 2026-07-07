using Content.IntegrationTests.Tests.Interaction;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests.Weapons;

public sealed class WeaponTests : InteractionTest
{
    protected override string PlayerPrototype => "MobHuman"; // The default test mob only has one hand
    private static readonly EntProtoId MobHuman = "MobHuman";
    private static readonly EntProtoId testWeapon = "NFWeaponRifleRepeater"; // Aurora's Song | changed from Mosin to repeater rifle, as new AS mosin functions differently and would cause a test failure.

    [Test]
    public async Task GunRequiresWieldTest()
    {
        var gunSystem = SEntMan.System<SharedGunSystem>();
        var damageSystem = SEntMan.System<DamageableSystem>();

        await AddAtmosphere(); // prevent the Urist from suffocating

        var urist = await SpawnTarget(MobHuman);
        var damageComp = Comp<DamageableComponent>(urist);

        var weapNet = await PlaceInHands(testWeapon);
        var weapEnt = ToServer(weapNet);

        await Pair.RunSeconds(2f); // Guns have a cooldown when picking them up.

        Assert.That(HasComp<GunRequiresWieldComponent>(weapNet),
            "Looks like you've removed the 'GunRequiresWield' component from the NFWeaponRifleRepeater." + // Aurora's Song | Change this if you change the test weapon
            "If this was intentional, please update WeaponTests.cs to reflect this change!");

        var startAmmo = gunSystem.GetAmmoCount(weapEnt);
        var wieldComp = Comp<WieldableComponent>(weapNet);

        Assert.That(startAmmo, Is.GreaterThan(0), "Weapon was spawned with no ammo!");
        Assert.That(wieldComp.Wielded, Is.False, "Weapon was spawned wielded!");

        await AttemptShoot(urist, false); // should fail due to not being wielded
        var updatedAmmo = gunSystem.GetAmmoCount(weapEnt);

        Assert.That(updatedAmmo,
            Is.EqualTo(startAmmo),
            "Weapon discharged ammo when the weapon should not have fired!");
        Assert.That(damageSystem.GetTotalDamage(ToServer(urist)),
            Is.EqualTo(FixedPoint2.Zero),
            "Urist took damage when the weapon should not have fired!");

        await UseInHand();

        Assert.That(wieldComp.Wielded, Is.True, "Weapon failed to wield when interacted with!");

        await AttemptShoot(urist);
        updatedAmmo = gunSystem.GetAmmoCount(weapEnt);

        Assert.That(updatedAmmo, Is.EqualTo(startAmmo - 1), "Weapon failed to discharge appropriate amount of ammo!");
        Assert.That(damageSystem.GetTotalDamage(ToServer(urist)),
            Is.GreaterThan(FixedPoint2.Zero),
            "Weapon was fired but urist sustained no damage!");
    }
}
