using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;

namespace Content.Shared.Power.Components;

/// <summary>
/// Handles <see cref="PoweredComponentComponent"/> component manipulation.
/// </summary>
public sealed class PoweredComponentSystem : EntitySystem
{
    [Dependency] private readonly SharedPowerReceiverSystem _receiver = default!;

    private void _receiver.TogglePower(Entity<PoweredComponentComponent> ent)
    {
        if (component.Powered && !component.Broken)
        {
            var target = ent.Comp.Parent ? Transform(ent).ParentUid : ent.Owner;

            if (TerminatingOrDeleted(target))
                return;

            ent.Comp.Target = target;

            EntityManager.AddComponents(target, ent.Comp.Components);
        }
        else
        {
            if (ent.Comp.Target == null)
                return;

            if (TerminatingOrDeleted(ent.Comp.Target.Value))
                return;

            EntityManager.RemoveComponents(ent.Comp.Target.Value, ent.Comp.RemoveComponents ?? ent.Comp.Components);
        }
    }
}
