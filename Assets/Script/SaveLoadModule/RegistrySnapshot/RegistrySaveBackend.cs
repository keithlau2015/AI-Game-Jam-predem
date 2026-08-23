using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using ItemModule;
using UnityEngine;

namespace SaveLoadModule.RegistrySnapshot
{
    /// <summary>
    /// Refactored original design: discover SaveableModel&lt;T&gt; maps, snapshot into SaveDataModel, one file per slot.
    /// </summary>
    public sealed class RegistrySaveBackend : ISaveBackend
    {
        private bool _catalogLoaded;
        private readonly List<SaveSlotInfo> _slots = new List<SaveSlotInfo>();

        public SaveBackendKind Kind => SaveBackendKind.RegistrySnapshot;

        public async UniTask EnsureCatalogLoaded()
        {
            if (_catalogLoaded)
                return;

            SaveDataModel.map.Clear();
            _slots.Clear();

            FileManager.EnsureInit();
            string dir = FileManager.filePath[(int)FileManager.FileType.Save];
            string[] files = FileManager.GetAllFileInDirectory(dir);
            if (files != null)
            {
                foreach (string fileName in files)
                {
                    if (string.IsNullOrEmpty(fileName) || !fileName.StartsWith(SaveDataModel.FilePrefix, StringComparison.Ordinal))
                        continue;

                    List<SaveDataModel> loaded =
                        await FileManager.LoadFile<SaveDataModel>(FileManager.FileType.Save, fileName, preserveTypeInfo: true);
                    if (loaded == null || loaded.Count == 0 || loaded[0] == null)
                        continue;

                    SaveDataModel model = loaded[0];
                    if (model.key == null)
                        model.key = fileName.Substring(SaveDataModel.FilePrefix.Length);

                    SaveDataModel.map[model.key] = model;
                }
            }

            RebuildSlotCache();
            _catalogLoaded = true;
        }

        public IReadOnlyList<SaveSlotInfo> ListSlots()
        {
            EnsureCatalogLoaded().GetAwaiter().GetResult();
            return _slots;
        }

        public async UniTask<string> CreateSave(string displayName)
        {
            await EnsureCatalogLoaded();

            SaveDataModel model = CaptureSnapshot(displayName);
            await PersistSlot(model);
            SaveDataModel.map[model.key] = model;
            RebuildSlotCache();
            return model.key?.ToString();
        }

        public void LoadSave(string slotId, out string errorCode)
        {
            errorCode = string.Empty;
            EnsureCatalogLoaded().GetAwaiter().GetResult();

            if (!SaveDataModel.map.TryGetValue(slotId, out SaveDataModel model) || model == null)
            {
                errorCode = ErrorCode.SaveFileCorrupted;
                return;
            }

            foreach (Type type in GetInheritedSaveableModel())
            {
                if (type == typeof(SaveDataModel))
                    continue;

                if (!model.data.TryGetValue(type.Name, out SortedDictionary<object, object> data) || data == null)
                    continue;

                MethodInfo loadMethod = type.GetMethod(
                    "Load",
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                    null,
                    new[] { typeof(List<>).MakeGenericType(type) },
                    null);

                if (loadMethod == null)
                {
                    Debug.LogWarning($"[RegistrySave] No Load(List<{type.Name}>) on {type.Name}");
                    continue;
                }

                object typedList = CreateTypedList(type, data.Values);
                try
                {
                    loadMethod.Invoke(null, new[] { typedList });
                }
                catch (Exception e)
                {
                    Debug.LogError($"[RegistrySave] Failed loading {type.Name}: {e}");
                    errorCode = ErrorCode.SaveFileCorrupted;
                    return;
                }
            }

            RebuildSideIndexes();

            if (!string.IsNullOrEmpty(model.LastLevelKey))
                PlayerPrefs.SetString("LastLevelKey", model.LastLevelKey);
        }

        public async UniTask DeleteSave(string slotId)
        {
            await EnsureCatalogLoaded();
            if (SaveDataModel.map.TryGetValue(slotId, out SaveDataModel model) && model != null)
            {
                FileManager.DeleteFile(FileManager.FileType.Save, model.FileName);
                SaveDataModel.map.Remove(slotId);
            }
            else
            {
                FileManager.DeleteFile(FileManager.FileType.Save, $"{SaveDataModel.FilePrefix}{slotId}");
            }

            RebuildSlotCache();
        }

        public async UniTask AutoSave()
        {
            int autoCount = SaveDataModel.map.Values.Count(x =>
                x != null && !string.IsNullOrEmpty(x.Name) && x.Name.StartsWith("AutoSave_", StringComparison.Ordinal));
            await CreateSave($"AutoSave_{autoCount}");
        }

        private static SaveDataModel CaptureSnapshot(string displayName)
        {
            SaveDataModel model = new SaveDataModel(displayName);
            foreach (Type type in GetInheritedSaveableModel())
            {
                if (type == typeof(SaveDataModel))
                    continue;

                PropertyInfo propertyInfo = GetMapProperty(type);
                if (propertyInfo == null)
                    continue;

                object data = propertyInfo.GetValue(null);
                if (data == null)
                    continue;

                if (data.GetType().IsGenericType
                    && data.GetType().GetGenericTypeDefinition() == typeof(SortedDictionary<,>))
                {
                    SortedDictionary<object, object> reconstructValue = new SortedDictionary<object, object>();
                    foreach (DictionaryEntry entry in (IDictionary)data)
                        reconstructValue[entry.Key] = entry.Value;

                    model.data[type.Name] = reconstructValue;
                }
            }

            model.Touch();
            model.PredictFileSize();
            return model;
        }

        private static async UniTask PersistSlot(SaveDataModel model)
        {
            FileManager.SaveFile(model, FileManager.FileType.Save, model.FileName, preserveTypeInfo: true);
            await UniTask.Yield();
        }

        private void RebuildSlotCache()
        {
            _slots.Clear();
            foreach (SaveDataModel model in SaveDataModel.map.Values.OrderByDescending(m => m.UpdatedUnixMs > 0 ? m.UpdatedUnixMs : m.CreateTime))
            {
                if (model == null)
                    continue;
                _slots.Add(model.ToSlotInfo());
            }
        }

        private static void RebuildSideIndexes()
        {
            ItemData.RebuildOwnerGroups();
        }

        private static PropertyInfo GetMapProperty(Type saveableType)
        {
            Type cursor = saveableType;
            while (cursor != null && cursor != typeof(object))
            {
                PropertyInfo propertyInfo = cursor.GetProperty(
                    "map",
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.DeclaredOnly);
                if (propertyInfo != null)
                    return propertyInfo;

                if (cursor.IsGenericType && cursor.GetGenericTypeDefinition() == typeof(Model<>))
                    return cursor.GetProperty("map", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                cursor = cursor.BaseType;
            }

            return saveableType.GetProperty("map", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        }

        private static object CreateTypedList(Type elementType, ICollection values)
        {
            Type listType = typeof(List<>).MakeGenericType(elementType);
            IList list = (IList)Activator.CreateInstance(listType);
            if (values == null)
                return list;

            foreach (object value in values)
            {
                if (value == null)
                    continue;
                if (elementType.IsInstanceOfType(value))
                    list.Add(value);
                else
                    Debug.LogWarning($"[RegistrySave] Skipping value of type {value.GetType().Name} for list<{elementType.Name}>");
            }

            return list;
        }

        private static IEnumerable<Type> GetInheritedSaveableModel()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
                })
                .Where(t =>
                    t.BaseType != null
                    && t.BaseType.IsGenericType
                    && t.BaseType.GetGenericTypeDefinition() == typeof(SaveableModel<>));
        }
    }
}
