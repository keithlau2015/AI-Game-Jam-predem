using AttributeModule;
using Cysharp.Threading.Tasks;
using EquipmentModule;
using Model;
using ObjetPoolModule;
using ProjectileModule;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace AbilityModule
{
    public class Skill
    {
        public ICombatUnit agent;
        protected bool isActive = true;
        public bool IsActive { get { return isActive; } }
        public bool isCoolingDown { get; protected set; }
        public int cd;
        public event Action<float> onCoolingDown;        
        public event Action<bool> onSkillActive;
        protected SkillModel model { get; set; }

        protected GameObject rangeVisualGO;

        private CancellationTokenSource cts = new CancellationTokenSource();

        private AttributeData _coolDown;
        private AttributeData _range;
        public AttributeData coolDown { get { return _coolDown; } }
        public AttributeData range { get { return _range; } }

        public List<GameObject> targets = new List<GameObject>();

        public Skill(ICombatUnit agent, SkillModel model)
        {
            this.agent = agent;
            this.model = model;
            this.cd = model.cd;

            this._coolDown = new AttributeData(this.model.cd);
            float a = model.targetRangeXValue / 2f;
            float b = model.targetRangeYValue / 2f;
            float area = Mathf.PI * a * b;
            float d = Mathf.Sqrt(area / Mathf.PI);
            this._range = new AttributeData(new System.Numerics.BigInteger(d));

            GameStateController.singleton.onPause += OnPause;
        }

        private void OnPause(bool isPause)
        {
            if (isPause)
            {
                StopCoolingDown();
            }
            else
            {
                if(this.cd < model.cd)
                    ResumeCoolingDown();
            }
        }

        public void SetActive(bool isActive)
        {            
            this.isActive = isActive;
            onSkillActive?.Invoke(isActive);
        }

        public void Execute()
        {
            if (model.type.Equals((int)SkillModel.SkillType.placement))
            {
                foreach (Transform transform in agent.GetProjectileAnchorList(this.model.key.ToString()))
                {
                    ProjectileModel projectileModel = null;
                    if(!ProjectileModel.map.TryGetValue(model.value, out projectileModel))
                    {
                        //ERROR
                        Debug.LogError($"Unable to find projectile key ${model.value}");
                        continue;
                    }

                    ObjectPool pool = null;
                    if (!ObjectPoolManager.singleton.pools.TryGetValue(projectileModel.entity, out pool))
                    {
                        //ERROR
                        Debug.LogError($"Unable to find pool key ${projectileModel.entity}");
                        continue;
                    }
                    GameObject placementObj = null;
                    if (placementObj == null)
                    {
                        placementObj = pool.SpawnFromPool();
                    }

                    //Handle projectile
                    Projectile projectile = null;
                    if (!placementObj.TryGetComponent(out projectile))
                    {
                        return;
                    }
                    placementObj.transform.position = transform.position;
                    projectile.SetUp(agent, projectileModel, this.model.formula);
                    Debug.Log($"Skill {model.key} firing projectile {projectileModel.key} from {transform.position} towards {targets.Count} targets");
                    foreach (GameObject gameObject in this.targets)
                    {
                        projectile.AddTarget(gameObject);
                    }
                    projectile.Emit(transform);
                }
            }
            else if (model.type.Equals((int)SkillModel.SkillType.buff))
            {
                List<string> buffKeys = SkillBuffModel.GetBuffModelKeyListBySkillKey(model.key.ToString());
                if (buffKeys == null || buffKeys.Count == 0)
                {
                    // Fallback: SkillModel.value as a single buff key
                    if (!string.IsNullOrEmpty(model.value))
                        buffKeys = new List<string> { model.value };
                }

                List<ICombatUnit> buffTargets = new List<ICombatUnit>();
                if (targets != null && targets.Count > 0)
                {
                    for (int i = 0; i < targets.Count; i++)
                    {
                        ICombatUnit unit = targets[i] != null
                            ? targets[i].GetComponentInParent<ICombatUnit>()
                            : null;
                        if (unit != null)
                            buffTargets.Add(unit);
                    }
                }
                else if (agent != null)
                {
                    buffTargets.Add(agent);
                }

                for (int t = 0; t < buffTargets.Count; t++)
                {
                    for (int b = 0; b < buffKeys.Count; b++)
                        BuffController.ApplyBuff(buffKeys[b], buffTargets[t]);
                }
            }

            /*
            List<GameObject> unselectTargets = targets.Where(x => {
                ICombatUnit targetAgent = x.GetComponent<ICombatUnit>();
                return targetAgent != null && !targetAgent.isAlive;
            }).ToList();
            */

            TryClearEquipmentAim();
            UnselectTarget(targets);
            ResetCoolingDown();
            StartCoolingDown();
        }

        private void TryClearEquipmentAim()
        {
            if (agent?.equipmentSlots == null || agent.equipmentSlots.Count == 0)
                return;

            int equipmentIndex = agent.GetEquipmentIndexBySkill(Model.key.ToString());
            if (equipmentIndex < 0 || equipmentIndex >= agent.equipmentSlots.Count)
                return;

            GameObject slotGo = agent.equipmentSlots[equipmentIndex];
            if (slotGo == null || slotGo.transform.childCount == 0)
                return;

            Transform equipmentTran = slotGo.transform.GetChild(0);
            AutoRotateToLockedTarget autoRotate = equipmentTran.GetComponent<AutoRotateToLockedTarget>();
            if (autoRotate != null)
                autoRotate.SetTarget(null);
        }

        public async void StartCoolingDown(int overrideCD = -2) {
            isCoolingDown = true;
            if (overrideCD == -1 || overrideCD > 0)
                this.cd = overrideCD;

            while(this.cd > 0 && !cts.IsCancellationRequested) {
                await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: false, cancellationToken: cts.Token);
                this.cd--;
                onCoolingDown?.Invoke(this.cd);
                Debug.Log($"Cooling Down: {cd}");
            }
            isCoolingDown = false;
        }

        public void StopCoolingDown() {
            cts.Cancel();
            cts = new CancellationTokenSource();
        }

        public void ResumeCoolingDown() {
            if (cts != null && cts.IsCancellationRequested)
            {
                cts.Dispose();
            }
            cts = new CancellationTokenSource();
            StartCoolingDown();
        }

        public void ResetCoolingDown() {
            this.cd = model.cd;
            if (cts != null)
            {
                cts.Dispose();
            }
            cts = new CancellationTokenSource();
            isCoolingDown = false;
        }

        public SkillModel Model { get { return model; } }

        public bool SelectTarget(List<GameObject> selectableObj)
        {
            if (targets.Count >= model.maxTarget) {
                Debug.LogError($"Skill {model.key} has reached max target limit of {model.maxTarget}");
                return false; 
            }

            if(agent.team.Equals(Team.Red))
                Debug.Log($"{agent}");

            foreach(GameObject gameObject in selectableObj){
                if (targets.Contains(gameObject)) continue;
                targets.Add(gameObject);
            }

            #region Visusal
            int equipmentIndex = this.agent.GetEquipmentIndexBySkill(Model.key.ToString());
            GameObject slotGo = this.agent.equipmentSlots[equipmentIndex];
            Transform equipmentTran = slotGo.transform.GetChild(0);
            AutoRotateToLockedTarget autoRotate = equipmentTran.GetComponent<AutoRotateToLockedTarget>();
            if (autoRotate && selectableObj.Count > 0)
            {
                autoRotate.SetTarget(selectableObj[0].transform);
            }
            else
            {
                Debug.LogWarning($"Skill {Model.key} does not have AutoRotateToLockedTarget component on equipment index {equipmentIndex}.");
            }
            /*
            int index = agent.GetEquipmentIndexBySkill(this.model.key.ToString());
            if (index >= 0 && index < agent.equipmentSlots.Count)
            {
                GameObject gameObj = agent.equipmentSlots[index];
                AutoRotateToLockedTarget autoRotate = gameObj.GetComponentInChildren<AutoRotateToLockedTarget>();
                if (autoRotate)
                {
                    autoRotate.SetTarget(selectableObj.FirstOrDefault().transform);
                }
                else
                {
                    Debug.LogWarning($"Skill {model.key} does not have AutoRotateToLockedTarget component on equipment index {index}.");
                }
            }
            else
            {
                Debug.LogWarning($"Skill {model.key} does not have a valid equipment index.");
            }
            */
            #endregion
            return true;
        }

        public bool UnselectTarget(List<GameObject> selectableObj)
        {
            int removeCount = targets.RemoveAll(target => selectableObj.Contains(target));
            if (removeCount == 0) return true;

            #region Visual
            int equipmentIndex = this.agent.GetEquipmentIndexBySkill(Model.key.ToString());
            GameObject slotGo = this.agent.equipmentSlots[equipmentIndex];
            Transform equipmentTran = slotGo.transform.GetChild(0);
            AutoRotateToLockedTarget autoRotate = equipmentTran.GetComponent<AutoRotateToLockedTarget>();
            if (autoRotate && targets.Count == 0)
            {
                autoRotate.SetTarget(null);
            }
            else
            {
                Debug.LogWarning($"Skill {Model.key} does not have AutoRotateToLockedTarget component on equipment index {equipmentIndex}.");
            }
            /*
            int index = agent.GetEquipmentIndexBySkill(this.model.key.ToString());
            if (index >= 0 && index < agent.equipmentSlots.Count)
            {
                GameObject gameObj = agent.equipmentSlots[index];
                AutoRotateToLockedTarget autoRotate = gameObj.GetComponentInChildren<AutoRotateToLockedTarget>();
                if (autoRotate)
                {
                    autoRotate.SetTarget(null);
                }
                else
                {
                    Debug.LogWarning($"Skill {model.key} does not have AutoRotateToLockedTarget component on equipment index {index}.");
                }
            }
            else
            {
                Debug.LogWarning($"Skill {model.key} does not have a valid equipment index.");
            }
            */
            #endregion
            return true;
        }

        public void PreviewRange() {
            ObjectPool pool = null;
            if (!ObjectPoolManager.singleton.pools.TryGetValue(model.rangeVisualEntityKey, out pool))
            {
                //ERROR
                Debug.LogError($"Range visual entity '{model.rangeVisualEntityKey}' not found in ObjectPoolManager");
                return;
            }
            rangeVisualGO = pool.SpawnFromPool();
            int equipmentIndex = agent.GetEquipmentIndexBySkill(Model.key.ToString());
            GameObject slotGo = agent.equipmentSlots[equipmentIndex];
            Transform equipmentTran = slotGo.transform.GetChild(0);
            rangeVisualGO.transform.SetParent(equipmentTran);
            rangeVisualGO.transform.localPosition = Vector3.zero;
            if(range.value != -1)
            {
                if(model.targetType.Equals((int)SkillModel.TargetType.direction))
                {
                    GameObject child = rangeVisualGO.transform.GetChild(0).gameObject;
                    RectTransform rectTransform = null;
                    if (!child.TryGetComponent(out rectTransform))
                    {
                        Debug.LogError($"Preview Range '{model.rangeVisualEntityKey}' does not have RectTransform component");
                        return;
                    }
                    rectTransform.sizeDelta = new Vector2(model.targetRangeXValue, model.targetRangeYValue);
                }
                else if(model.targetType.Equals((int)SkillModel.TargetType.assignable))
                {
                    GameObject child = rangeVisualGO.transform.GetChild(0).gameObject;
                    RectTransform rectTransform = null;
                    if (!child.TryGetComponent(out rectTransform))
                    {
                        Debug.LogError($"Preview Range '{model.rangeVisualEntityKey}' does not have RectTransform component");
                        return;
                    }

                    rectTransform.sizeDelta = new Vector2(model.targetRangeXValue, rectTransform.sizeDelta.y);
                    Animator animator = null;
                    if (!child.TryGetComponent(out animator))
                    {
                        Debug.LogError($"Preview Range '{model.rangeVisualEntityKey}' does not have Animator component");
                        return;
                    }

                    // ?i?J???w???A?A?�u]?m??S?w?V
                    animator.speed = 0;
                    float normalizedTime = model.targetRangeYValue / 360f;
                    animator.Play("ATKRange_circle", 0, normalizedTime);
                    animator.Update(0); // ??Y????
                }
            }
        }

        public void CancelPreviewRange() {
            if(rangeVisualGO != null) {
                rangeVisualGO.transform.SetParent(null);
                rangeVisualGO.SetActive(false);
            }
        }

        public bool IsPreviewingRange () {
            return rangeVisualGO != null && rangeVisualGO.activeSelf;
        }

        public void AimRangeVisual(Transform aimTransform)
        {
            if (rangeVisualGO == null || aimTransform == null)
                return;

            if (!rangeVisualGO.TryGetComponent(out FacingToAim facing))
                return;

            facing.SetTarget(aimTransform);
        }
    }
}