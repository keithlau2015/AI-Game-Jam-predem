using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using SaveLoadModule;
using System;
using System.Diagnostics;
using System.Linq;

namespace Model
{
    public class AchievementHistoryModel : SaveableModel<AchievementHistoryModel>
    {
        private object _achievementKey;
        public object achievementKey
        {
            get
            {
                return this._achievementKey;
            }

            protected set
            {
                //set value
                this._achievementKey = value;
                //add to map
                if (!mapByModel.ContainsValue(this) && !mapByModel.ContainsKey(_achievementKey))
                {
                    mapByModel.Add(_achievementKey, this);
                }
                //try observe value
                _model = null;
                if (!AchievementModel.map.TryGetValue(achievementKey, out _model)) return;
                //subscribed guard
                if (subscribed) return;
                //subscribe progress
                Type modelType = Type.GetType(_model.observeModel);
                Debug.Assert(modelType != null);
                if (modelType == null) return;
                //get map
                object mapObj = modelType.GetProperty("map").GetValue(null);
                IDictionary targetDict = (IDictionary)mapObj;
                if (targetDict.Contains(_model.achieveKey))
                {
                    var modelObj = targetDict[_model.achieveKey];
                    
                    if (typeof(INotifyPropertyChanged).IsAssignableFrom(modelType))
                    {
                        INotifyPropertyChanged observeable = (INotifyPropertyChanged)modelObj;
                        observeable.PropertyChanged += Observe;
                        this.subscribed = true;
                    }
                }
            }
        }

        private bool _isAchieved;
        public bool IsAchieved
        {
            get
            {
                return this._isAchieved;
            }
            protected set
            {
                this._isAchieved = value;
            }
        }

        public static SortedDictionary<object, AchievementHistoryModel> mapByModel = new SortedDictionary<object, AchievementHistoryModel>();
        private AchievementModel _model;
        private bool subscribed = false;
        //Load
        public AchievementHistoryModel(object key) : base(key) { }

        //Create
        public AchievementHistoryModel(string achievementKey) : base()
        {
            this.achievementKey = achievementKey;
        }

        public static void ClearSideIndexes()
        {
            mapByModel.Clear();
        }

        public static AchievementHistoryModel FromSave(object key, object achievementKey)
        {
            AchievementHistoryModel model = new AchievementHistoryModel(key);
            model.achievementKey = achievementKey;
            return model;
        }

        private void Observe(Object sender, PropertyChangedEventArgs e)
        {
            if (_model == null)
            {
                if (!AchievementModel.map.TryGetValue(key, out _model))
                {
                    return;
                }
            }

            Type modelType = Type.GetType(_model.observeModel);
            Debug.Assert(modelType != null);
            if (modelType != null && sender.GetType().IsEquivalentTo(modelType))
            {
                if (e.PropertyName == _model.observeField)
                {
                    //check pre achievement is finished
                    if (_model.preAchievementKey != null)
                    {
                        AchievementHistoryModel histModel = null;
                        if (mapByModel.TryGetValue(_model.preAchievementKey, out histModel))
                        {
                            if (!histModel.IsAchieved) return;
                        }
                    }
                    //if is achieved
                    if (this.IsAchieved)
                    {
                        if (sender is INotifyPropertyChanged)
                        {
                            INotifyPropertyChanged observeable = (INotifyPropertyChanged)sender;
                            observeable.PropertyChanged -= Observe;
                            return;
                        }
                    }

                    var value = modelType.GetProperty(e.PropertyName).GetValue(sender);
                    this.IsAchieved = value.Equals(_model.achieveValue);
                }
            }
        }
    }
}