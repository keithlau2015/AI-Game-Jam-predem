using BehaviorTree;
using EPOOutline;
using AttributeModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using LocalizationModule;
using AbilityModule;
using System.Linq;
using Model;
using EquipmentModule;
using ObjetPoolModule;
using System.Numerics;

namespace CombatUnitModule
{
    public class CombatUnitAgent : PoolObjectProperty, ISelectable, IDestructible, ICombatUnit
    {
        #region Behavior Tree
        [SerializeField]
        protected TreeExecutor genericBattleUnit;
        #endregion

        #region Unity Components
        [SerializeField]
        protected NavMeshAgent _agent;
        public NavMeshAgent agent { get { return _agent; } }
        #endregion

        #region Display
        [SerializeField]
        protected AttributeUI hpUI;
        [SerializeField]
        protected Image miniMapIcon;
        [SerializeField]
        protected GameObject shieldGO;
        public GameObject targetIndicator;
        [SerializeField]
        protected Outlinable outlinable;
        [SerializeField]
        protected LineRenderer pathRenderer;
        protected GameObject destIndictor;
        [SerializeField, ColorUsage(true, true)]
        protected Color selectedOutlineColor;
        protected Color originOutlineColor;
        #endregion

        #region Equipment
        public List<GameObject> equipmentSlots = new List<GameObject>();
        List<GameObject> ICombatUnit.equipmentSlots => equipmentSlots;
        public List<CombatUnitEquipmentSlotData> slots = new List<CombatUnitEquipmentSlotData>();
        public List<Transform> GetProjectileAnchorList(string skillKey)
        {
            int index = GetEquipmentIndexBySkill(skillKey);
            GameObject equipmentGo = equipmentSlots[index];
            if (equipmentGo == null)
            {
                Debug.LogError($"Equipment slot {index} is null.");
                return null;
            }
            Equipment equipment = equipmentGo.GetComponentInChildren<Equipment>();
            if (equipment == null)
            {
                Debug.LogError($"Equipment slot {index} does not have an Equipment component.");
                return null;
            }
            return equipment.ProjectileAnchor;
        }

        #endregion

        #region Skill
        public Dictionary<int, List<Skill>> skillMap = new Dictionary<int, List<Skill>>(); //key slot index, value skill list
        public int GetEquipmentIndexBySkill(string skillKey)
        {
            var slot = slots.FirstOrDefault(x =>
                x.equipmentData != null &&
                EquipmentSkillModel.GetSkillModelListByEquipmentKey(x.equipmentData.id.ToString()) is var skillModels &&
                skillModels != null &&
                skillModels.Any(y => y.key.ToString() == skillKey)
            );

            Debug.Log($"GetEquipmentIndexBySkill: skillKey={skillKey}, slot={slot?.equipmentData?.id}, index={slots.IndexOf(slot)}");

            return slot != null ? slots.IndexOf(slot) : -1;
        }
        #endregion

        #region Status
        public bool showTargetIndicator { get; private set; } = false;
        public Team team { get; private set; }

        public bool isAlive
        {
            get
            {
                AttributeData hpIns = null;
                if (!attributes.TryGetValue((int)AttributeModel.AttributeType.HP, out hpIns))
                    return false;

                return hpIns.value > hpIns.minValue;
            }
        }

        public virtual void OnHoverIn()
        {
            outlinable.enabled = true;
        }

        public virtual void OnHoverOut()
        {
            if (isSelected)
                return;

            outlinable.enabled = false;
        }

        public virtual void OnSelect()
        {
            if (!outlinable.enabled)
                outlinable.enabled = true;
            isSelected = true;
            this.originOutlineColor = outlinable.OutlineParameters.Color;
            outlinable.OutlineParameters.Color = this.selectedOutlineColor;
            showTargetIndicator = true;
        }

        public virtual void OnDeselect()
        {
            outlinable.enabled = false;
            isSelected = false;
            outlinable.OutlineParameters.Color = this.originOutlineColor;
            showTargetIndicator = false;
        }

        public GameObject GetGameObject()
        {
            return this.gameObject;
        }

        private void OnHpDamage(int dir, BigInteger diff, BigInteger value, BigInteger maxValue)
        {
            GameObject go = ObjectPoolManager.singleton.pools["3"].SpawnFromPool();
            AttributeNumPopUpUI attributeNumPopUpUI = null;
            if (!go.TryGetComponent(out attributeNumPopUpUI))
                return;

            attributeNumPopUpUI.SetUp(new Color(1, 0.6679427f, 0, 1), $"{diff}", this.transform.position, this.transform.position + new UnityEngine.Vector3(0, 10, 0));
        }

        private void OnShieldDamage(int dir, BigInteger diff, BigInteger value, BigInteger maxValue)
        {
            GameObject go = ObjectPoolManager.singleton.pools["3"].SpawnFromPool();
            AttributeNumPopUpUI attributeNumPopUpUI = null;
            if (!go.TryGetComponent(out attributeNumPopUpUI))
                return;

            attributeNumPopUpUI.SetUp(new Color(0, 0.6366434f, 1, 1), $"{diff}", this.transform.position, this.transform.position + new UnityEngine.Vector3(0, 10, 0));
            //Reset shield regen cool down
            CancelShieldCoolDown();
            StartShieldCoolDown();
        }

        private void OnDead()
        {
            this.gameObject.SetActive(false);
            CinemachineShake.singleton.ShakeCamera(3f, 1.5f);
        }

        private void OnShieldDown()
        {
            //await UniTask.Delay(3000);
            //this.shieldGO.SetActive(false);
            //CinemachineShake.singleton.ShakeCamera(1f, 1.5f);
            isShieldActive = false;
        }

        public List<CombatUnitAgent> prioritytargets = new List<CombatUnitAgent>();
        #endregion

        #region Utility
        public static List<CombatUnitAgent> allUnitEntities { get; private set; } = new List<CombatUnitAgent>();
        public List<CombatUnitAgent> teammates
        {
            get
            {
                return allUnitEntities.FindAll(x => x.team == this.team);
            }
        }
        public int index { get; private set; }
        public bool isSelected { get; private set; } = false;
        public bool isInit { get; private set; } = false;
        private CombatUnitModel model;
        public CombatUnitModel unitModel
        {
            get
            {
                return model;
            }

            private set
            {
                model = value;
            }
        }

        public void CopyComponents(CombatUnitAgent combatUnitAgent)
        {
            this._agent = combatUnitAgent._agent;
            this.hpUI = combatUnitAgent.hpUI;
            this.targetIndicator = combatUnitAgent.targetIndicator;
            this.outlinable = combatUnitAgent.outlinable;
            this.selectedOutlineColor = combatUnitAgent.selectedOutlineColor;
            this.originOutlineColor = combatUnitAgent.originOutlineColor;
            this.pathRenderer = combatUnitAgent.pathRenderer;
            this.destIndictor = combatUnitAgent.destIndictor;
            this.miniMapIcon = combatUnitAgent.miniMapIcon;
            this.equipmentSlots.AddRange(combatUnitAgent.equipmentSlots);
            this.genericBattleUnit = combatUnitAgent.genericBattleUnit;
        }

        #endregion

        public virtual async void SetUp(CombatUnitData ins)
        {
            #region Equipments & skills
            slots = ins.slotList;
            for (int i = 0; i < slots.Count; i++)
            {
                CombatUnitEquipmentSlotData equipmentSlotData = slots[i];
                EquipmentData equipmentData = equipmentSlotData.GetEquipmentData();
                if (equipmentData == null)
                {
                    //ERROR
                    Debug.LogError($"No equipment found for key: {equipmentSlotData.equipmentUID}");
                    continue;
                }
                EquipmentModel equipmentModel = equipmentData.GetEquipmentModel();
                if (equipmentModel == null)
                {
                    //ERROR
                    Debug.LogError($"No equipment found for key: {equipmentData.id.ToString()}");
                    continue;
                }
                ItemModel itemModel = equipmentModel.GetItemModel();
                if (itemModel == null)
                {
                    //ERROR
                    Debug.LogError($"No item found for key: {equipmentModel.item}");
                    continue;
                }

                ObjectPool pool = itemModel.GetObjectPool();
                if (pool == null)
                {
                    //ERROR
                    Debug.LogError($"No pool found for key: {itemModel.entityID}");
                    continue;
                }

                GameObject equipmentGO = pool.SpawnFromPool();
                Equipment equipment = null;
                if (!equipmentGO.TryGetComponent(out equipment))
                {
                    //ERROR
                    Debug.LogError($"No equipment componet: {equipmentGO.name}");
                    continue;
                }
                equipmentGO.transform.SetParent(equipmentSlots[i].transform);
                equipmentGO.transform.localPosition = UnityEngine.Vector3.zero;

                List<SkillModel> skillModels = EquipmentSkillModel.GetSkillModelListByEquipmentKey(equipmentData.id.ToString());
                if (skillModels == null || skillModels.Count == 0)
                {
                    Debug.LogWarning($"No skill found for equipment: {equipmentData.id.ToString()}");
                    continue;
                }
                Debug.Log($"Found {skillModels.Count} skills for equipment: {equipmentData.id.ToString()}");
                List<Skill> skills = new List<Skill>();
                foreach (SkillModel skillModel in skillModels)
                {
                    Skill skill = new Skill(this, skillModel);
                    if (skill != null)
                    {
                        skills.Add(skill);
                    }
                    else
                    {
                        Debug.LogError($"Failed to create skill: {skillModel.key}");
                    }
                }
                equipment.SetUp(skills);
                skillMap.Add(i, skills);
            }

            Debug.Log($"Total equipments: {equipmentSlots.Count}, Total skills: {skillMap.Sum(x => x.Value.Count)}");
            Debug.Log($"Equipment slots: {string.Join(", ", equipmentSlots.Select((slot, idx) => $"{idx}:{(slot.transform.childCount > 0 ? slot.transform.GetChild(0).name : "Empty")}"))}");
            Debug.Log($"Skill map: {string.Join(", ", skillMap.Select(kv => $"{kv.Key}:[{string.Join(", ", kv.Value.Select(s => s.Model.key.ToString()))}]"))}");
            #endregion

            #region Init Attribute Ins
            //Create tmp Ins
            AttributeData hpIns = new AttributeData(ins.hp, ins.maxHp);
            attributes.Add((int)AttributeModel.AttributeType.HP, hpIns);

            hpIns.onValueMin += () =>
            {
                OnDead();
            };

            hpIns.onValuePostChange += (dir, diff, value, maxValue) =>
            {
                if (dir == -1)
                    OnHpDamage(dir, diff, value, maxValue);
            };

            AttributeData atkIns = new AttributeData(ins.atk);
            attributes.Add((int)AttributeModel.AttributeType.ATK, atkIns);
            AttributeData defIns = new AttributeData(ins.def);
            attributes.Add((int)AttributeModel.AttributeType.DEF, defIns);
            AttributeData mpIns = new AttributeData(ins.spd);
            attributes.Add((int)AttributeModel.AttributeType.MP, mpIns);
            AttributeData criIns = new AttributeData(ins.cri);
            attributes.Add((int)AttributeModel.AttributeType.CRI, criIns);
            AttributeData criDmgIns = new AttributeData(ins.criDmg);
            attributes.Add((int)AttributeModel.AttributeType.CRI_DMG, criDmgIns);
            AttributeData dodgeIns = new AttributeData(ins.dodge);
            attributes.Add((int)AttributeModel.AttributeType.DODGE, dodgeIns);
            AttributeData spdIns = new AttributeData(ins.spd);
            attributes.Add((int)AttributeModel.AttributeType.SPD, spdIns);
            AttributeData inspectRangeIns = new AttributeData(ins.inspectRange);
            attributes.Add((int)AttributeModel.AttributeType.INSPECT_RANGE, inspectRangeIns);
            AttributeData counterInspectRangeIns = new AttributeData(ins.counterInspectRange);
            attributes.Add((int)AttributeModel.AttributeType.COUNTER_INSPECT_RANGE, counterInspectRangeIns);
            AttributeData shieldIns = new AttributeData(ins.shield, ins.maxShield);
            attributes.Add((int)AttributeModel.AttributeType.SHIELD, shieldIns);
            AttributeData hitIns = new AttributeData(ins.hit);
            attributes.Add((int)AttributeModel.AttributeType.HIT, hitIns);
            AttributeData shieldEffIns = new AttributeData(ins.shieldEfficiency);
            attributes.Add((int)AttributeModel.AttributeType.SHIELD_EFFICIENCY, shieldEffIns);
            shieldIns.onValueMin += () =>
            {
                OnShieldDown();
            };

            shieldIns.onValuePostChange += (dir, diff, value, maxValue) =>
            {
                if (dir > 0 && !isShieldActive && value >= (maxValue * new System.Numerics.BigInteger(0.5f)))
                {
                    //Regen Shield Visual Effect
                    GameObject go = ObjectPoolManager.singleton.pools["8"].SpawnFromPool();
                    go.transform.position = this.transform.position;
                    isShieldActive = true;
                }

                if (dir == -1)
                    OnShieldDamage(dir, diff, value, maxValue);
            };

            AttributeData shieldRegenSpdIns = new AttributeData(ins.shieldRegenSpd);
            attributes.Add((int)AttributeModel.AttributeType.SHIELD_REGEN_SPD, shieldRegenSpdIns);

            //Assign Speed to _agent
            _agent.speed = (float)spdIns.value;
            #endregion

            GameStateController.singleton.onPause += (isPause) =>
            {
                if (!this.gameObject.activeSelf)
                    return;

                if (!_agent.enabled)
                    return;

                if (isPause)
                    _agent.isStopped = true;
                else
                    _agent.isStopped = false;
            };

            targetIndicator.SetActive(false);
            hpUI.SetUp(hpIns);

            if (!allUnitEntities.Contains(this))
                allUnitEntities.Add(this);
            this.team = ins.isPlayer ? Team.Blue : Team.Red;
            CombatUnitModel.map.TryGetValue(ins.id, out model);

            if (genericBattleUnit != null)
                genericBattleUnit.ConstructTree();

            this.index = index;

            #region mini map
            miniMapIcon.sprite = await GameAssetsBundleManager.LoadSprite($"Icon_shipLv{(model.type > 0 ? model.type : 1)}");
            miniMapIcon.color = team == Team.Blue ? new Color(0.2065237f, 0.4443313f, 0.8584906f) : new Color(0.764151f, 0.3695917f, 0.3568441f);
            #endregion
            isInit = true;
        }

        #region Attack Action related (obsolete)
        public void StartAttackCoolDown()
        {
            if (!this.isAlive || GameStateController.singleton.IsPause)
                return;

            if (attackCoolDownCoroutine != null)
            {
                StopCoroutine(attackCoolDownCoroutine);
            }

            attackCoolDownCoroutine = StartCoroutine(AttackCoolDown());
        }

        public void CancelAttackCoolDown()
        {
            AttributeData coolDownIns = null;
            if (attributes.TryGetValue((int)AttributeModel.AttributeType.ATK_SPD, out coolDownIns))
                attackCoolDownTime = Convert.ToInt32(coolDownIns.value);

            if (attackCoolDownCoroutine != null)
            {
                StopCoroutine(attackCoolDownCoroutine);
                attackCoolDownCoroutine = null;
            }
        }

        private IEnumerator AttackCoolDown()
        {
            AttributeData coolDownIns = null;
            if (attributes.TryGetValue((int)AttributeModel.AttributeType.ATK_SPD, out coolDownIns))
                attackCoolDownTime = Convert.ToInt32(coolDownIns.value);
            while (attackCoolDownTime > 0)
            {
                if (GameStateController.singleton.IsPause)
                {
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(1);
                    attackCoolDownTime--;
                }
            }
        }
        #endregion

        #region Shield Action related
        public bool isShieldActive { get; private set; } = true;
        public int attackCoolDownTime { get; private set; } = 0;
        public int shieldCoolDownTime { get; private set; } = 0;

        private Dictionary<int, AttributeData> _attributes = new Dictionary<int, AttributeData>();
        public Dictionary<int, AttributeData> attributes
        {
            get
            {
                return _attributes;
            }
        }

        private Coroutine shieldCoolDownCoroutine;
        private Coroutine attackCoolDownCoroutine;
        public void StartShieldCoolDown()
        {
            if (!this.isAlive || GameStateController.singleton.IsPause)
                return;

            if (shieldCoolDownCoroutine != null)
            {
                StopCoroutine(shieldCoolDownCoroutine);
            }

            shieldCoolDownTime = 5;
            shieldCoolDownCoroutine = StartCoroutine(ShieldCoolDown());
        }

        public void CancelShieldCoolDown()
        {
            if (shieldCoolDownCoroutine != null)
            {
                StopCoroutine(shieldCoolDownCoroutine);
                shieldCoolDownCoroutine = null;
            }
            shieldCoolDownTime = 5;
        }

        private IEnumerator ShieldCoolDown()
        {
            while (shieldCoolDownTime > 0)
            {
                if (GameStateController.singleton.IsPause)
                {
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(1);
                    shieldCoolDownTime--;
                }
            }
        }
        #endregion

        public string UnitId
        {
            get { return unitModel != null && unitModel.key != null ? unitModel.key.ToString() : name; }
        }

        public string GetName()
        {
            return $"{this.unitModel.nameID}: {LocalizationManager.singleton.GetLocalization(this.unitModel.nameID)}" + (team == Team.Blue ? " - Player" : "");
        }

        protected override void OnDisable()
        {
            foreach (var attr in attributes.Values)
            {
                attr.Clean();
            }

            attributes.Clear();
            attackCoolDownTime = 0;
            outlinable.enabled = false;
            isSelected = false;
            isInit = false;
            showTargetIndicator = false;
            targetIndicator.SetActive(false);
            StopAllCoroutines();
            base.OnDisable();
        }

        public void OnDestruct()
        {

        }

        public void OnRepair()
        {

        }

        public void OnHit(System.Numerics.BigInteger dmg)
        {

        }
    }
}