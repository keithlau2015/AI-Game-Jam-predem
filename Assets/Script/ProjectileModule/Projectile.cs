using AttributeModule;
using FormulaModule;
using Model;
using ObjetPoolModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace ProjectileModule
{
    public class Projectile : PoolObjectProperty, IDestructible
    {
        [SerializeField]
        private string projectileID;

        private ProjectileModel model;
        private Queue<GameObject> targets = new Queue<GameObject>();
        private float delay;
        private Coroutine followObjectCoroutine;
        private AttributeModule.AttributeData _spdAttr;
        private AttributeModule.AttributeData _hpAttr;
        private ICombatUnit emitBy;
        public event Action<Projectile, Collider> onTriggerEnter;
        public int formulaId;
        public void SetUp(ICombatUnit agent, ProjectileModel projectileModel, int formulaId)
        {
            this.model = projectileModel;
            this.projectileID = projectileModel.key.ToString();
            this.emitBy = agent;
            if (!ProjectileModel.map.TryGetValue(projectileID, out this.model))
                return;

            _spdAttr = new AttributeData(new BigInteger(model.spd));
            _hpAttr = new AttributeData(new BigInteger(model.hp));
            this.formulaId = formulaId;
        }

        private AttributeModule.AttributeData spdAttr
        {
            get
            {
                if (_spdAttr == null)
                {
                    _spdAttr = new AttributeData(new BigInteger(model.spd));
                }

                return _spdAttr;
            }
        }

        private AttributeModule.AttributeData hpAttr
        {
            get
            {
                if (_hpAttr == null)
                {
                    _hpAttr = new AttributeData(new BigInteger(model.hp));
                }

                return _hpAttr;
            }
        }

        public void AddTarget(GameObject target)
        {
            if (!target.activeSelf) return;
            this.targets.Enqueue(target);
        }

        private IEnumerator FollowObject(float delay)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log($"projectile[{model.name}] start follow object");
            GameObject curTarget = targets.Dequeue();
            while (UnityEngine.Vector3.Distance(this.transform.position, curTarget.transform.position) > 0.1)
            {
                Debug.Log($"projectile[{model.name}] in range moving to target");
                transform.LookAt(curTarget.transform);
                transform.position = UnityEngine.Vector3.MoveTowards(transform.position, curTarget.transform.position, (float)spdAttr.value);
                yield return null;
            }

            StopCoroutine(followObjectCoroutine);
            if (hpAttr.value > hpAttr.minValue)
            {
                Debug.Log($"projectile[{model.name}] still have hp start another loop for following object");
                followObjectCoroutine = StartCoroutine(FollowObject(delay));
            }
        }

        public void Emit(Transform dir = null)
        {
            if (model.isTracker == 0)
            {
                Debug.Log($"projectile[{model.name}] is not tracker, move forward directly");
                // Create leading target at the correct position (at the projectile's position)
                //GameObject leadingTarget = new GameObject("LeadingTarget");
                //GameObject leadingTarget = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //leadingTarget.transform.position = transform.position + transform.forward * 100f;

                // Get mouse position in world space
                /*
                Vector2 mousePosition2D = Mouse.current.position.ReadValue();
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePosition2D.x, mousePosition2D.y, 0));
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
                if (groundPlane.Raycast(ray, out float distance))
                {
                    Vector3 worldMousePosition = ray.GetPoint(distance);

                    // Calculate direction vector from projectile to mouse position
                    Vector3 direction = worldMousePosition - transform.position;

                    // Keep movement on X-Z plane
                    direction.y = 0;
                    direction.Normalize();

                    // Set rotation to face the mouse position
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    // Apply rotation to both projectile and leading target
                    transform.rotation = targetRotation;
                    leadingTarget.transform.rotation = targetRotation;

                    Debug.Log($"Mouse Position: {worldMousePosition}, Direction: {direction}, Target Rotation: {targetRotation.eulerAngles}");
                }
                */
                if (dir != null)
                {
                    UnityEngine.Quaternion targetRotation = UnityEngine.Quaternion.LookRotation(dir.forward);
                    // Apply rotation to both projectile and leading target
                    transform.rotation = targetRotation;
                    //leadingTarget.transform.rotation = targetRotation;
                }

                MoveForward moveForward = gameObject.AddComponent<MoveForward>();
                moveForward.speed = (float)spdAttr.value;
                //SelfDestruction projectileSD = null;
                //if (!gameObject.TryGetComponent(out projectileSD))
                //{
                //    SelfDestruction leadingSelfDestruction = gameObject.AddComponent<SelfDestruction>();
                //    leadingSelfDestruction.lifeTime = projectileSD.lifeTime;
                //    leadingSelfDestruction.hardDestroy = false;
                //    leadingSelfDestruction.StartCountingDown();
                //}
                //AddTarget(leadingTarget);
            }
            else
            {
                Debug.Log($"projectile[{model.name}] is tracker, start follow object after {delay} seconds");
                followObjectCoroutine = StartCoroutine(FollowObject(delay));
            }

            //Handle life time
            SelfDestruction selfDestruction = null;
            if (!this.TryGetComponent(out selfDestruction))
            {
                return;
            }
            selfDestruction.lifeTime = model.lifeTime;
            if (selfDestruction.isAutoStart)
                selfDestruction.StartCountingDown();

            GameStateController.singleton.onPause += OnPause;
        }

        private void OnPause(bool isPause)
        {
            if (isPause)
            {
                if (followObjectCoroutine != null)
                {
                    StopCoroutine(followObjectCoroutine);
                }
            }
            else
            {
                if (model.isTracker != 0) return;
                followObjectCoroutine = StartCoroutine(FollowObject(delay));
            }
        }

        public bool CheckEmitBy(ICombatUnit combatUnit)
        {
            return emitBy != null && emitBy.Equals(combatUnit);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other || emitBy == null) return;

            GameObject emitGo = emitBy.GetGameObject();
            if (other != null && other.CompareTag("Hitable") && emitGo != null && !other.gameObject.Equals(emitGo))
            {
                //check projectile emitBy;         
                Projectile projectile = null;
                if (other.gameObject.TryGetComponent(out projectile))
                {
                    //if the projectile is emitted by the same unit, ignore the hit
                    if (projectile.CheckEmitBy(emitBy)) return;
                }

                Debug.Log($"Projectile detected hit! {other.name}");
                if (model.isTracker != 0)
                {
                    StopCoroutine(followObjectCoroutine);
                }
                Impact(other);
            }
        }

        protected override void OnDisable()
        {
            MoveForward moveForward = null;
            if (this.TryGetComponent(out moveForward))
            {
                Destroy(moveForward);
            }

            SelfDestruction selfDestruction = null;
            if (!this.TryGetComponent(out selfDestruction))
            {
                return;
            }
            Debug.Log($"Projectile {this.name} disabled, emitBy: {emitBy?.GetName()}, selfDestruction: {selfDestruction.lifeTime}");

            selfDestruction.StopAllCoroutines();

            emitBy = null;

            base.OnDisable();
        }

        private void Impact(Collider other)
        {
            ICombatUnit targetUnitEntity = null;
            if (other != null)
                targetUnitEntity = other.GetComponentInParent<ICombatUnit>();
            if (targetUnitEntity == null)
            {
                Debug.LogError("Projectile Impact: target has no ICombatUnit.");
                return;
            }

            if (targetUnitEntity.Equals(this.emitBy))
            {
                Debug.Log("Projectile Impact: target is emitBy, ignoring self-hit.");
                return;
            }

            AttributeData shieldIns = null;
            if (!targetUnitEntity.attributes.TryGetValue((int)AttributeModel.AttributeType.SHIELD, out shieldIns))
            {
                Debug.Log("Projectile Impact: target has no shield attribute.");
                return;
            }

            AttributeData hpIns = null;
            if (!targetUnitEntity.attributes.TryGetValue((int)AttributeModel.AttributeType.HP, out hpIns))
            {
                Debug.Log("Projectile Impact: target has no HP attribute.");
                return;
            }

            BigInteger dmg = FormulaController.GetDmg(emitBy, targetUnitEntity, this.formulaId);
            Debug.Log($"dmg: {dmg}");

            GameObject targetGo = targetUnitEntity.GetGameObject();
            BigInteger overflowValue = 0;
            if (shieldIns.value > 0 && targetUnitEntity.isShieldActive)
            {
                overflowValue = shieldIns.SetValue(-dmg, AttributeData.EditMode.Add);
                if (overflowValue > 0 || shieldIns.value == 0)
                {
                    GameObject go = ObjectPoolManager.singleton.pools["7"].SpawnFromPool();
                    go.transform.position = transform.position;
                }
                else
                {
                    GameObject go = ObjectPoolManager.singleton.pools["6"].SpawnFromPool();
                    UnityEngine.Vector3 hitPoint = other.ClosestPoint(transform.position);
                    UnityEngine.Vector3 factHitPoint = UnityEngine.Quaternion.LookRotation(hitPoint).eulerAngles;
                    go.transform.eulerAngles = new UnityEngine.Vector3(factHitPoint.x, 0, factHitPoint.y);
                    if (targetGo != null)
                        go.transform.position = targetGo.transform.position;
                }
                GameLog.logger.Log($"{(targetUnitEntity.team == Team.Blue ? "<color=#0394fc>" : "<color=#ff4d4d>")}{targetUnitEntity.GetName()}[{targetUnitEntity.UnitId}]</color> hit by projectile costing {dmg} damage to shield");
            }
            else
            {
                hpIns.SetValue(-dmg, AttributeData.EditMode.Add);
                GameLog.logger.Log($"{(targetUnitEntity.team == Team.Blue ? "<color=#0394fc>" : "<color=#ff4d4d>")}{targetUnitEntity.GetName()}[{targetUnitEntity.UnitId}]</color> hit by projectile costing {dmg} damage to hull");
            }
            Debug.Log($"overflow: {overflowValue}");

            if (overflowValue > 0 && hpIns.value > 0)
            {
                hpIns.SetValue(-overflowValue, AttributeData.EditMode.Add);
                GameLog.logger.Log($"{(targetUnitEntity.team == Team.Blue ? "<color=#0394fc>" : "<color=#ff4d4d>")}{targetUnitEntity.GetName()}[{targetUnitEntity.UnitId}]</color> hit by projectile break through shield costing {overflowValue} damage to hull");
            }


            //Handle explosion (optional)
            if (!string.IsNullOrEmpty(this.model.explosionEntityKey))
            {
                ObjectPool pool = null;
                if (!ObjectPoolManager.singleton.pools.TryGetValue(model.explosionEntityKey, out pool))
                {
                    //ERROR
                }
                GameObject explosionGO = pool.SpawnFromPool();
                Explosion explosion = null;
                if (!explosionGO.TryGetComponent(out explosion))
                {
                    return;
                }
            }
            Debug.Log("Projectile Impact, setting to active false");
            gameObject.SetActive(false);
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