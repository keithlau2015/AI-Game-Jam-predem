using EquipmentModule;
using GenericGameModule;
using Model;
using ObjetPoolModule;
using SaveLoadModule;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CombatUnitModule
{
    public class CombatUnitData : SaveableModel<CombatUnitData>
    {
        public bool isPlayer { get; set; } = false;
        public object id { get; private set; }

        #region Attributes
        public int maxHp { get; private set; }
        public int hp { get; private set; }
        public int atk { get; private set; }
        public int def { get; private set; }
        public int hit { get; private set; }
        public int dodge { get; private set; }
        public int cri { get; private set; }
        public int criDmg { get; private set; }
        public int spd { get; private set; }
        public int inspectRange { get; private set; }
        public int counterInspectRange { get; private set; }
        public int shield { get; private set; }
        public int maxShield {  get; private set; }
        public int shieldRegenSpd {  get; private set; }
        public int shieldEfficiency { get; private set; }
        #endregion

        public List<CombatUnitEquipmentSlotData> slotList = new List<CombatUnitEquipmentSlotData>();

        //Create
        public CombatUnitData(object id, bool isPlayer) : base()
        {
            this.id = id;
            this.isPlayer = isPlayer;
        }

        public CombatUnitData(object key) : base(key)
        {
        }

        public static void ClearPlayerShipCache()
        {
            _playerShip = null;
        }

        /// <summary>Restore persisted combat unit stats (DocumentDto / tools).</summary>
        public static CombatUnitData FromSave(
            object key,
            object id,
            bool isPlayer,
            int maxHp, int hp, int atk, int def, int hit, int dodge, int cri, int criDmg, int spd,
            int inspectRange, int counterInspectRange, int shield, int maxShield, int shieldRegenSpd, int shieldEfficiency)
        {
            CombatUnitData data = new CombatUnitData(key);
            data.id = id;
            data.isPlayer = isPlayer;
            data.maxHp = maxHp;
            data.hp = hp;
            data.atk = atk;
            data.def = def;
            data.hit = hit;
            data.dodge = dodge;
            data.cri = cri;
            data.criDmg = criDmg;
            data.spd = spd;
            data.inspectRange = inspectRange;
            data.counterInspectRange = counterInspectRange;
            data.shield = shield;
            data.maxShield = maxShield;
            data.shieldRegenSpd = shieldRegenSpd;
            data.shieldEfficiency = shieldEfficiency;
            if (isPlayer)
                _playerShip = data;
            return data;
        }

        private static CombatUnitData _playerShip;
        public static CombatUnitData playerShip
        {
            get
            {
                if (_playerShip == null)
                    _playerShip = new CombatUnitData("1", true);

                return _playerShip;
            }
        }


        private void InitValue(CombatUnitModel model)
        {
            this.hp = model.hp;
            this.maxHp = model.maxHp;
            this.atk = model.atk;
            this.def = model.def;
            this.hit = model.hit;
            this.dodge = model.dodge;
            this.cri = model.cri;
            this.criDmg = model.criDmg;
            this.spd = model.spd;
            this.inspectRange = model.inspectRange;
            this.counterInspectRange = model.counterInspectRange;
            this.shield = model.shield;
            this.maxShield = model.maxShield;
            this.shieldRegenSpd = model.shieldRegenSpd;
            this.shieldEfficiency = model.armorEfficiency;

            List<CombatUnitEquipmentSlotData> slotDataList = CombatUnitEquipmentSlotData.GetSlotsByUnitUID(this.key.ToString());
            List<EquipmentData> equipmentList = new List<EquipmentData>();
            if(slotDataList == null || slotDataList.Count == 0)
            {
                List<CombatUnitEquipmentSlotModel> slotModelList = CombatUnitEquipmentSlotModel.GetModelListByUnit(id.ToString());
                int slotIndex = 0;
                foreach (var slotModel in slotModelList)
                {
                    slotIndex++;
                    if (string.IsNullOrEmpty(slotModel.defaultEquipmentId))
                    {
                        continue;
                    }

                    EquipmentData equipmentData = new EquipmentData(slotModel.defaultEquipmentId, this.key.ToString());
                    CombatUnitEquipmentSlotData slotData = new CombatUnitEquipmentSlotData(this.id.ToString(), equipmentData.key.ToString(), slotIndex);

                    this.hp += equipmentData.GetEquipmentModel().hp;
                    this.maxHp += equipmentData.GetEquipmentModel().maxHp;
                    this.atk += equipmentData.GetEquipmentModel().atk;
                    this.def += equipmentData.GetEquipmentModel().def;
                    this.hit += equipmentData.GetEquipmentModel().hit;
                    this.dodge += equipmentData.GetEquipmentModel().dodge;
                    this.cri += equipmentData.GetEquipmentModel().cri;
                    this.criDmg += equipmentData.GetEquipmentModel().criDmg;
                    this.spd += equipmentData.GetEquipmentModel().spd;
                    this.inspectRange += equipmentData.GetEquipmentModel().inspectRange;
                    this.counterInspectRange += equipmentData.GetEquipmentModel().counterInspectRange;
                    this.shield += equipmentData.GetEquipmentModel().shield;
                    this.maxShield += equipmentData.GetEquipmentModel().maxShield;
                    this.shieldRegenSpd += equipmentData.GetEquipmentModel().shieldRegenSpd;
                    this.shieldEfficiency += equipmentData.GetEquipmentModel().armorEfficiency;

                    slotList.Add(slotData);
                    equipmentList.Add(equipmentData);
                }
            }
            else
            {
                foreach (var slotData in slotDataList)
                {
                    if (slotData.equipmentUID != null && EquipmentData.mapByOwner.TryGetValue(slotData.equipmentUID, out var equipmentDataList))
                    {
                        equipmentList.AddRange(equipmentDataList);
                    }
                }
            }
        }

        public GameObject InstantiateEntity()
        {
            //Get Skin
            CombatUnitModel model = null;
            if (!CombatUnitModel.map.TryGetValue(id, out model))
                return null;

            InitValue(model);

            EntityModel entityModel = null;
            if (!EntityModel.map.TryGetValue(model.entityID, out entityModel))
                return null;

            List<SkinModel> skinDSList = null;
            if (!SkinModel.mapByEntity.TryGetValue((string)entityModel.key, out skinDSList))
            {
                Debug.Log("Instantiate Entity: No skin data found");
            }
            else
            {
                skinDSList.Sort((x, y) =>
                {
                    if (x.sortIndex < y.sortIndex)
                        return 1;
                    else if (x.sortIndex > y.sortIndex)
                        return -1;
                    else
                        return 0;
                });
            }

            //Instantiate gameobject
            GameObject prefab = ObjectPoolManager.singleton.pools[entityModel.key.ToString()].SpawnFromPool();
            Assert.IsNotNull(prefab, "Instantiate Entity: Failed to spawn entity from pool for key " + entityModel.key.ToString());
            SkinController skinController = prefab.GetComponentInChildren<SkinController>();
            if (skinController && skinDSList != null)
                skinController.SetSkinByID((string)skinDSList[entityModel.defaultSkinIndex].key);

            CombatUnitAgent combatUnitAgent = null;
            if (this.isPlayer)
            {
                if (prefab.TryGetComponent(out combatUnitAgent))
                {
                    PlayerCombatUnitAgent playerBattleAgent = prefab.AddComponent<PlayerCombatUnitAgent>();
                    playerBattleAgent.CopyComponents(combatUnitAgent);
                    GameObject.Destroy(combatUnitAgent);
                    combatUnitAgent = playerBattleAgent;
                }
            }
            else
            {
                if (!prefab.TryGetComponent(out combatUnitAgent))
                {
                    prefab.AddComponent<CombatUnitAgent>();
                }
            }
            combatUnitAgent.SetUp(this);

            return prefab;
        }
    }
}