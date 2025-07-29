using GameUI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using GenericGameModule;
using SaveLoadModule;
using LocalizationModule;
using AttributeSystem;
using ItemModule;
using Unity.VisualScripting;

public class GameManager : Singleton<GameManager>
{
    private const int TOTAL_SYS_USE = 10;

    protected async void Start()
    {
        //Set Loading data set(Show Total Progress & Sub Progress)
        LoadingManager.singleton.Show(true, TOTAL_SYS_USE);

        #region Start up procedure
        //Step 1: Load all local data set       

        // Load SaveFile
        IProgress<int> saveProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Loading Saves...", 1);
        await FileManager.LoadFile<SaveDataModel>();
        saveProgress.Report(1);

        //Localization
        IProgress<int> localizationProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[1]", 1);
        await FileManager.LoadEncryptedModel<LocalizationModel>();
        localizationProgress.Report(1);

        //Attribute
        IProgress<int> attributeProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[2]", 1);
        await FileManager.LoadFile<AttributeData>();
        attributeProgress.Report(1);

        //Skin
        IProgress<int> skinProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[3]", 2);
        await FileManager.LoadEncryptedModel<SkinModel>();
        skinProgress.Report(1);
        await FileManager.LoadFile<SkinData>();
        skinProgress.Report(2);

        //Item
        IProgress<int> itemProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[4]", 2);
        await FileManager.LoadEncryptedModel<ItemModel>();
        itemProgress.Report(1);
        await FileManager.LoadFile<ItemData>();
        itemProgress.Report(2);

        //Equipment
        IProgress<int> equipmentProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Retrieving data...[5]", 3);
        await FileManager.LoadEncryptedModel<EquipmentModel>();
        equipmentProgress.Report(1);
        await FileManager.LoadFile<EquipmentData>();
        equipmentProgress.Report(2);

        //Object Pooling
        List<EntityModel> objectDSList = await FileManager.LoadEncryptedModel<EntityModel>();
        IProgress<int> objectPoolProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Preloading entities...", EntityModel.mapByPrefabKey.Count + 1);
        ObjectPoolManager.singleton.SetUp(objectPoolProgress);
        #endregion

        //Landing Panel UI
        IProgress<int> loginPanelUIProgress = await LoadingManager.singleton.AddTask(LoadingManager.PresentType.ShowPercentage, "Loading ui...", 1);
        LandingPanel landingPanel = await UIManager.singleton.LoadUI<LandingPanel>(typeof(LandingPanel).Name);
        loginPanelUIProgress.Report(1);
        LoadingManager.singleton.Hide();
    }

    public static void ExitGame()
    {

    }

    public static void SaveAll()
    {

    }

    public static void DestoryAllChild(GameObject root)
    {
        if (root == null)
            return;
        for (int i = 0; i < root.transform.childCount; i++)
        {
            Destroy(root.transform.GetChild(i).gameObject);
        }
    }

    public static void ChangeAllLayer(GameObject root, string layerName)
    {
        if (root == null)
            return;
        int layer = LayerMask.NameToLayer(layerName);
        root.layer = layer;
        for (int i = 0; i < root.transform.childCount; i++)
        {
            root.transform.GetChild(i).gameObject.layer = layer;
        }
    }

#if UNITY_EDITOR
    private static void AddDefinition(string definition)
    {
        var group = EditorUserBuildSettings.selectedBuildTargetGroup;
        var definitions = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, definitions + ";" + definition);
    }

    private static void RemoveDefinition(string definition)
    {
        var group = EditorUserBuildSettings.selectedBuildTargetGroup;
        var definitions = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        var splited = definitions.Split(';');

        var builder = new StringBuilder();

        var addedCount = 0;
        foreach (var item in splited)
        {
            if (item == definition)
                continue;

            builder.Append(item);
            builder.Append(';');
            addedCount++;
        }

        if (addedCount != 0)
            builder.Remove(builder.Length - 1, 1);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, builder.ToString());
    }
#endif
}