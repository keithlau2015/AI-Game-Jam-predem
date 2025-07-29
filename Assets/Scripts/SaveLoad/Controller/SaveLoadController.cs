using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace SaveLoadModule
{
    public class SaveLoadController
    {
        private static int autoSaveCount
        {
            get
            {
                return SaveDataModel.map.Values.Where(x => { 
                    SaveDataModel model = x as SaveDataModel;
                    return model.Name.Contains("AutoSave_");
                }).Count();
            }
        }

        public static void LoadSave(string uid, out string errorCode)
        {
            errorCode = string.Empty;
            SaveDataModel model = null;
            if (!SaveDataModel.map.TryGetValue(uid, out model))
            {
                errorCode = ErrorCode.SaveFileCorrupted;
                return;
            }

            foreach (Type type in GetInheritedSaveableModel())
            {
                MethodInfo method = type.GetMethod("Load");
                MethodInfo generic = method.MakeGenericMethod(type);
                SortedDictionary<object, object> data = null;
                if (!model.data.TryGetValue(type.Name, out data)) continue;
                method.Invoke(null, data.Values.ToArray());
            }
        }


        public static void CreateSave(string name)
        {
            SaveDataModel model = new SaveDataModel(name);
            foreach (Type type in GetInheritedSaveableModel())
            {             
                PropertyInfo propertyInfo = type.BaseType.BaseType.GetProperty("map");
                if (propertyInfo == null) continue;
                object data = propertyInfo.GetValue(null);             
                if (data == null) continue;
                if (data.GetType().IsGenericType && data.GetType().GetGenericTypeDefinition() == typeof(SortedDictionary<,>))
                {
                    SortedDictionary<object, object> reconstructValue = new SortedDictionary<object, object>();
                    foreach (DictionaryEntry entry in (IDictionary)data)
                    {
                        reconstructValue.Add(entry.Key, entry.Value);
                    }

                    if (!model.data.ContainsKey(type.Name))
                    {
                        model.data.Add(type.Name, reconstructValue);
                    }
                }
            }
            model.PredictFileSize();
            FileManager.SaveFile(SaveDataModel.map.ToArray(), FileManager.FileType.Save, name);
        }

        public static void DeleteSave(string name)
        {
            FileManager.DeleteFile(FileManager.FileType.Save, name);
        }

        public static void AutoSave()
        {
            string autoSaveName = $"AutoSave_{autoSaveCount}";
            CreateSave(autoSaveName);
        }

        private static IEnumerable<Type> GetInheritedSaveableModel()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType != null && t.BaseType.IsGenericType &&
                t.BaseType.GetGenericTypeDefinition() == typeof(SaveableModel<>));
        }
    }
}