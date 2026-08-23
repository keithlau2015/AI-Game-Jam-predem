using Model;

namespace AchievementModule
{
    public class AchievementController
    {
        public bool IsAchieved(object key)
        {
            AchievementHistoryModel histModel = null;
            if (!AchievementHistoryModel.mapByModel.TryGetValue(key, out histModel)) return false;
            return histModel.IsAchieved;
        }

        public bool IsChainAchieved(object key)
        {
            AchievementModel model = null;
            AchievementHistoryModel histModel = null;
            if (!AchievementModel.map.TryGetValue(key, out model)) return false;
            while (model.preAchievementKey != null)
            {
                if (!AchievementHistoryModel.mapByModel.TryGetValue(model.preAchievementKey, out histModel)) break;
                if (!histModel.IsAchieved) return false;
                if (!AchievementModel.map.TryGetValue(model.preAchievementKey, out model)) break;
            }
            return true;
        }

        //Param key: needed to be the last achievement key
        public int GetTotalChainCount(object key)
        {
            AchievementModel model = null;
            if (!AchievementModel.map.TryGetValue(key, out model)) return 0;
            int count = 1;
            while (model.preAchievementKey != null)
            {
                count++;
                if (!AchievementModel.map.TryGetValue(model.preAchievementKey, out model)) break;
            }
            return count;
        }

        //Param key: needed to be the last achievement key
        public int GetChainProgress(object key)
        {
            AchievementModel model = null;
            AchievementHistoryModel histModel = null;
            int count = 0;
            if (!AchievementModel.map.TryGetValue(key, out model)) return count;
            while (model.preAchievementKey != null)
            {
                if (!AchievementHistoryModel.mapByModel.TryGetValue(model.preAchievementKey, out histModel)) break;
                if (histModel.IsAchieved) count++;
                if (!AchievementModel.map.TryGetValue(model.preAchievementKey, out model)) break;
            }
            return count;
        }
    }
}