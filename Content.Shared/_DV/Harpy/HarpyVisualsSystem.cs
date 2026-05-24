using Content.Shared.Inventory.Events;
// using Content.Shared.Tag; // Frontier
using Content.Shared.Humanoid;
using Content.Shared._NF.Clothing.Components;
using Content.Shared.Inventory; // Frontier

namespace Content.Shared._DV.Harpy;

public sealed class HarpyVisualsSystem : EntitySystem
{
    // [Dependency] private readonly TagSystem _tagSystem = default!; // Frontier
    [Dependency] private readonly SharedHideableHumanoidLayersSystem _humanoidSystem = default!; // Aurora's Song

    //    [ValidatePrototypeId<TagPrototype>] // Frontier
    //    private const string HarpyWingsTag = "HidesHarpyWings"; // Frontier

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HarpySingerComponent, DidEquipEvent>(OnDidEquipEvent);
        SubscribeLocalEvent<HarpySingerComponent, DidUnequipEvent>(OnDidUnequipEvent);
    }

    private void OnDidEquipEvent(EntityUid uid, HarpySingerComponent component, DidEquipEvent args)
    {
        if (args.Slot == "outerClothing" && HasComp<HarpyHideWingsComponent>(args.Equipment)) // Frontier: Swap tag to comp
        {
            _humanoidSystem.SetLayerVisibility(uid, HumanoidVisualLayers.RArmExtension, false, SlotFlags.OUTERCLOTHING); // Frontier: RArm<RArmExtension
            _humanoidSystem.SetLayerVisibility(uid, HumanoidVisualLayers.Tail, false, SlotFlags.OUTERCLOTHING);
        }
    }

    private void OnDidUnequipEvent(EntityUid uid, HarpySingerComponent component, DidUnequipEvent args)
    {
        if (args.Slot == "outerClothing" && HasComp<HarpyHideWingsComponent>(args.Equipment)) // Frontier: Swap tag to comp
        {
            _humanoidSystem.SetLayerVisibility(uid, HumanoidVisualLayers.RArmExtension, true, SlotFlags.OUTERCLOTHING); // Frontier: RArm<RArmExtension
            _humanoidSystem.SetLayerVisibility(uid, HumanoidVisualLayers.Tail, true, SlotFlags.OUTERCLOTHING);
        }
    }
}
