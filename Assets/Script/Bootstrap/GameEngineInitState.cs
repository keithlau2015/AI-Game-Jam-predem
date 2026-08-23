using AttributeModule;
using CombatUnitModule;
using EquipmentModule;
using GameUI;
using GenericGameModule;
using ItemModule;
using LocalizationModule;
using Model;
using ObjetPoolModule;
using SaveLoadModule;
using System;

/// <summary>
/// Bootloader init: load data catalogs, warm object pools, then show LandingPanel.
/// Adapted from Sky_Garden for the Null template (CombatUnit* types + SaveService).
/// </summary>
public class GameEngineInitState : GameState
{
    private const int TOTAL_SYS_USE = 13;

    public GameEngineInitState(GameStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override async void OnEnter()
    {
        LoadingManager.singleton.Show(true, TOTAL_SYS_USE);

        IProgress<int> saveProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Loading Saves...", 1);
        await SaveService.EnsureCatalogLoaded();
        saveProgress.Report(1);

        IProgress<int> localizationProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[1]", 1);
        await FileManager.LoadEncryptedModel<LocalizationModel>();
        localizationProgress.Report(1);

        IProgress<int> attributeProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[2]", 1);
        await FileManager.LoadFile<AttributeData>();
        attributeProgress.Report(1);

        IProgress<int> skinProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[3]", 2);
        await FileManager.LoadEncryptedModel<SkinModel>();
        skinProgress.Report(1);
        await FileManager.LoadFile<SkinData>();
        skinProgress.Report(2);

        IProgress<int> itemProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[4]", 2);
        await FileManager.LoadEncryptedModel<ItemModel>();
        itemProgress.Report(1);
        await FileManager.LoadFile<ItemData>();
        itemProgress.Report(2);

        IProgress<int> equipmentProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[5]", 3);
        await FileManager.LoadEncryptedModel<EquipmentModel>();
        equipmentProgress.Report(1);
        await FileManager.LoadEncryptedModel<EquipmentSkillModel>();
        equipmentProgress.Report(2);
        await FileManager.LoadFile<EquipmentData>();
        equipmentProgress.Report(3);

        IProgress<int> slotProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[6]", 2);
        await FileManager.LoadEncryptedModel<CombatUnitEquipmentSlotModel>();
        slotProgress.Report(1);
        await FileManager.LoadFile<CombatUnitEquipmentSlotData>();
        slotProgress.Report(2);

        IProgress<int> unitProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[7]", 1);
        await FileManager.LoadEncryptedModel<CombatUnitModel>();
        unitProgress.Report(1);

        IProgress<int> skillProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[8]", 1);
        await FileManager.LoadEncryptedModel<SkillModel>();
        skillProgress.Report(1);

        IProgress<int> projectileProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[9]", 1);
        await FileManager.LoadEncryptedModel<ProjectileModel>();
        projectileProgress.Report(1);

        IProgress<int> levelProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[10]", 1);
        await FileManager.LoadEncryptedModel<LevelModel>();
        levelProgress.Report(1);

        IProgress<int> evtProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[11]", 3);
        await FileManager.LoadEncryptedModel<EvtModel>();
        evtProgress.Report(1);
        await FileManager.LoadEncryptedModel<EvtTriggerModel>();
        evtProgress.Report(2);
        await FileManager.LoadEncryptedModel<EvtEvtTriggerModel>();
        evtProgress.Report(3);

        _ = await FileManager.LoadEncryptedModel<EntityModel>();
        IProgress<int> objectPoolProgress = await LoadingManager.singleton.AddTask(
            LoadingManager.PresentType.ShowPercentage,
            "Preloading entities...",
            Math.Max(1, EntityModel.mapByPrefabKey.Count + 1));
        ObjectPoolManager.singleton.SetUp(objectPoolProgress);

        IProgress<int> loginPanelUIProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Loading ui...", 1);
        _ = await UIManager.singleton.LoadUI<LandingPanel>(typeof(LandingPanel).Name);
        loginPanelUIProgress.Report(1);
        LoadingManager.singleton.Hide();
    }

    public override void OnExit()
    {
    }

    public override void OnLogicUpdate()
    {
    }

    public override void OnPhysicsUpdate()
    {
    }
}
