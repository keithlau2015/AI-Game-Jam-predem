using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace CombatUnitModule
{
    public class CaptureableAgent : PoolObjectProperty
    {
        [SerializeField]
        private int id;
        [SerializeField]
        private Text progressLabel;
        [SerializeField]
        private Text teamLabel;
        public bool isInit { get; private set; } = false;

        private List<CombatUnitAgent> occupyingUnitEntities = new List<CombatUnitAgent>();
        private List<CombatUnitAgent> occupyingPlayerUnitEntities
        {
            get
            {
                return occupyingUnitEntities.FindAll(x => x.team == Team.Blue);
            }
        }
        private List<CombatUnitAgent> occupyingNPCUnitEntities
        {
            get
            {
                return occupyingUnitEntities.FindAll(x => x.team == Team.Red);
            }
        }

        public List<CaptureableAgent> allCaptureableEntities { get; private set; } = new List<CaptureableAgent>();
        public Dictionary<int, int> teamCaptureProgressMap { get; private set; } = new Dictionary<int, int>();
        public event Action<int, int> OnUnitEntityCapturing;

        //public void Load(BattleCaptureableData battleDomainData)
        //{
        //    if (isInit)
        //        return;
        //
        //    this.battleCaptureableData = battleDomainData;
        //    if(!allCaptureableEntities.Contains(this))
        //        allCaptureableEntities.Add(this);
        //    isInit = true;
        //    StartCoroutine(CheckOccupy());
        //}

        public void New(string battleUID, int id)
        {
            if (isInit)
                return;

            if (!allCaptureableEntities.Contains(this))
                allCaptureableEntities.Add(this);
            isInit = true;
            StartCoroutine(CheckOccupy());
        }

        private IEnumerator CheckOccupy()
        {
            while (isInit)
            {
                yield return new WaitWhile(() => GameStateController.singleton.IsPause);

                foreach (CombatUnitAgent unitEntity in CombatUnitAgent.allUnitEntities)
                {
                    if (unitEntity == null) { continue; }

                    if (Vector3.Distance(unitEntity.transform.position, transform.position) <= 50f)
                    {
                        if (!occupyingUnitEntities.Contains(unitEntity))
                            occupyingUnitEntities.Add(unitEntity);
                    }
                    else
                    {
                        if (occupyingUnitEntities.Contains(unitEntity))
                            occupyingUnitEntities.Remove(unitEntity);
                    }
                }

                if (occupyingPlayerUnitEntities.Count == 0 && occupyingNPCUnitEntities.Count == 0)
                    yield return new WaitForEndOfFrame();

                if (occupyingNPCUnitEntities.Count == occupyingPlayerUnitEntities.Count)
                    yield return new WaitForEndOfFrame();


                if (teamCaptureProgressMap[(int)Team.Blue] == 100)
                {
                    teamLabel.text = "Blue Team";
                    teamLabel.color = Color.cyan;
                }
                else if (teamCaptureProgressMap[(int)Team.Red] == 100)
                {
                    teamLabel.text = "Red Team";
                    teamLabel.color = Color.red;
                }
                else
                {
                    teamLabel.text = "";
                    teamLabel.color = Color.white;
                }

                if (occupyingPlayerUnitEntities.Count > occupyingNPCUnitEntities.Count)
                {
                    if (teamCaptureProgressMap[(int)Team.Red] > 0)
                    {
                        teamCaptureProgressMap[(int)Team.Red]--;
                        if (teamCaptureProgressMap[(int)Team.Red] < 0)
                            teamCaptureProgressMap[(int)Team.Red] = 0;

                        progressLabel.text = $"{teamCaptureProgressMap[(int)Team.Red]}";
                        progressLabel.color = Color.red;
                        yield return new WaitForSeconds(1);
                        OnUnitEntityCapturing?.Invoke((int)Team.Red, teamCaptureProgressMap[(int)Team.Red]);
                    }
                    else if (teamCaptureProgressMap[(int)Team.Blue] < 100)
                    {
                        teamCaptureProgressMap[(int)Team.Blue]++;
                        if (teamCaptureProgressMap[(int)Team.Blue] > 100)
                            teamCaptureProgressMap[(int)Team.Blue] = 100;

                        progressLabel.text = $"{teamCaptureProgressMap[(int)Team.Blue]}";
                        progressLabel.color = Color.cyan;
                        yield return new WaitForSeconds(1);
                        OnUnitEntityCapturing?.Invoke((int)Team.Blue, teamCaptureProgressMap[(int)Team.Blue]);
                    }
                }
                else if (occupyingNPCUnitEntities.Count > occupyingPlayerUnitEntities.Count)
                {
                    if (teamCaptureProgressMap[(int)Team.Blue] > 0)
                    {
                        teamCaptureProgressMap[(int)Team.Blue]--;
                        if (teamCaptureProgressMap[(int)Team.Blue] < 0)
                            teamCaptureProgressMap[(int)Team.Blue] = 0;

                        progressLabel.text = $"{teamCaptureProgressMap[(int)Team.Blue]}";
                        progressLabel.color = Color.cyan;
                        yield return new WaitForSeconds(1);
                        OnUnitEntityCapturing?.Invoke((int)Team.Blue, teamCaptureProgressMap[(int)Team.Blue]);
                    }
                    else if (teamCaptureProgressMap[(int)Team.Red] < 100)
                    {
                        teamCaptureProgressMap[(int)Team.Red]++;
                        if (teamCaptureProgressMap[(int)Team.Red] > 100)
                            teamCaptureProgressMap[(int)Team.Red] = 100;

                        progressLabel.text = $"{teamCaptureProgressMap[(int)Team.Red]}";
                        progressLabel.color = Color.red;
                        yield return new WaitForSeconds(1);
                        OnUnitEntityCapturing?.Invoke((int)Team.Red, teamCaptureProgressMap[(int)Team.Red]);
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}