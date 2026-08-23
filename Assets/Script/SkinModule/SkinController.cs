using System.Collections.Generic;
using UnityEngine;
using GenericGameModule;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class SkinController : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer skinnedMeshRenderer;
    public bool isIniting { get; private set; }

    public async void SetSkinByIns(string skinInsUID)
    {
        isIniting = true;
        this.transform.root.gameObject.SetActive(false);
        GameObject miniLoading = LoadingManager.singleton.ShowMini(UIManager.topLayer);

        SkinData skinInstance = null;
        if (!SkinData.map.TryGetValue(skinInsUID, out skinInstance))
            return;

        SkinModel skinDS = null;
        if (!SkinModel.map.TryGetValue(skinInstance.id, out skinDS))
            return;

        Mesh mesh = await AssetsBundleManager.LoadMesh(skinDS.meshID);

        Material material = null;
        string materialKey = null;
        if (skinDS.MaterialsIDList().Count == 0 || skinDS.MaterialsIDList() == null)
        {
            Debug.LogError("SkinDS Materials is Empty!");
            return;
        } 

        materialKey = skinDS.MaterialsIDList()[skinInstance.materialIndex];
        material = await AssetsBundleManager.LoadMaterial(materialKey);

        SetSkinMeshRenderer(mesh, material);

        this.transform.root.gameObject.SetActive(true);
        LoadingManager.singleton.HideMini(miniLoading);
        isIniting = false;
    }

    public async void SetSkinByID(string skinID)
    {
        isIniting = true;
        this.transform.root.gameObject.SetActive(false);
        GameObject miniLoading = LoadingManager.singleton.ShowMini(UIManager.topLayer);

        SkinModel skinDS = null;
        if (!SkinModel.map.TryGetValue(skinID, out skinDS))
            return;

        Mesh mesh = await AssetsBundleManager.LoadMesh(skinDS.meshID);

        Material material = null;
        string materialKey = null;
        if (skinDS.MaterialsIDList().Count == 0 || skinDS.MaterialsIDList() == null)
        {
            Debug.LogError("SkinDS Materials is Empty!");
            return;
        }

        materialKey = skinDS.MaterialsIDList()[skinDS.defaultMaterialIndex];
        material = await AssetsBundleManager.LoadMaterial(materialKey);

        SetSkinMeshRenderer(mesh, material);

        this.transform.root.gameObject.SetActive(true);
        LoadingManager.singleton.HideMini(miniLoading);
        isIniting = false;
    }

    private void SetSkinMeshRenderer(Mesh mesh, Material material)
    {
        if (skinnedMeshRenderer == null)
        {
            //Get Required Component
            if (!TryGetComponent(out skinnedMeshRenderer))
            {
                skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
            }
        }

        //Set Component
        skinnedMeshRenderer.sharedMesh = mesh;
        skinnedMeshRenderer.materials[0] = material;
    }
}
