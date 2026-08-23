using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class FileManager
{
    public enum FileType
    {
        Config,
        Save,
        Log
    }

    private static byte[] keyArray;
    private static byte[] ivArray;

    private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    public static event Action onFinishSaveObj;

    public static string[] filePath { get; private set; } = new string[3];
    public static bool isInit { get; private set; } = false;

    private static RijndaelManaged rijndaelManaged;

    public static void EnsureInit()
    {
        if (!isInit)
            Init();
    }

    private static void Init()
    {
        filePath[0] = $"{Application.dataPath}/Resources/";
        filePath[1] = $"{Application.persistentDataPath}/Game/";
        filePath[2] = $"{Application.persistentDataPath}/GameLog/";

        ResolveCryptoKeys();

        rijndaelManaged = new RijndaelManaged();
        rijndaelManaged.Key = keyArray;
        rijndaelManaged.IV = ivArray;

        #region Type Converter
        #region Tuple 2
        TypeDescriptor.AddAttributes(typeof((System.Int32, System.Int32)), new TypeConverterAttribute(typeof(TupleConverter.TwoParam<System.Int32, System.Int32>)));
        TypeDescriptor.AddAttributes(typeof((System.Int64, System.Int64)), new TypeConverterAttribute(typeof(TupleConverter.TwoParam<System.Int64, System.Int64>)));
        #endregion

        #region Tuple 3
        TypeDescriptor.AddAttributes(typeof((System.Int32, System.Int32, System.Int32)), new TypeConverterAttribute(typeof(TupleConverter.ThreeParam<System.Int32, System.Int32, System.Int32>)));
        TypeDescriptor.AddAttributes(typeof((System.Int64, System.Int64, System.Int64)), new TypeConverterAttribute(typeof(TupleConverter.ThreeParam<System.Int64, System.Int64, System.Int64>)));
        #endregion
        #endregion

        isInit = true;
    }

    private static void ResolveCryptoKeys()
    {
        FileManagerCryptoConfig config = Resources.Load<FileManagerCryptoConfig>("FileManagerCryptoConfig");
        string key;
        string iv;

        if (config != null && !string.IsNullOrEmpty(config.aesKey) && !string.IsNullOrEmpty(config.aesIv))
        {
            key = config.aesKey;
            iv = config.aesIv;
            if (config.IsPlaceholder)
                Debug.LogWarning("[FileManager] Using placeholder crypto keys from FileManagerCryptoConfig. Rotate before shipping.");
        }
        else
        {
            // Dev fallback so existing encrypted project data still loads until a config asset is added.
            key = "57_dQ0<fd~1dCMNfjc8_dp09m>kiytT0";
            iv = "a_03Mkd<[~sDtC02";
            Debug.LogWarning("[FileManager] Resources/FileManagerCryptoConfig missing. Using built-in dev fallback keys. Create the asset and rotate keys before shipping.");
        }

        keyArray = UTF8Encoding.UTF8.GetBytes(key);
        ivArray = UTF8Encoding.UTF8.GetBytes(iv);
    }


    public static bool CheckFileIsExits(string fileName)
    {
        return File.Exists($"{fileName}");
    }

    public static string[] GetAllFileInDirectory(string path, string[] excludeExtension = null)
    {
        if (!Directory.Exists(path))
            return null;
        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        FileInfo[] files = directoryInfo.GetFiles();
        List<string> allFilesName = new List<string>();
        foreach (FileInfo fileInfo in files)
        {
            bool isExclude = false;
            if (excludeExtension != null)
            {
                foreach (string ext in excludeExtension)
                {
                    if (fileInfo.Extension.Equals(ext))
                    {
                        isExclude = true;
                        break;
                    }

                }
            }
            if (isExclude)
                continue;
            allFilesName.Add(fileInfo.Name);
        }
        return allFilesName.ToArray();
    }

    public static async void SaveFile<T>(T[] objs, FileType fileType, string fileName = "", bool preserveTypeInfo = false)
    {
        if (!isInit)
            Init();

        await CreateFile<T>(fileType, fileName, objs, preserveTypeInfo);
    }

    public static async void SaveFile<T>(T obj, FileType fileType, string fileName, bool preserveTypeInfo = false)
    {
        if (!isInit)
            Init();

        if (obj == null)
            return;

        T[] objArray = { obj };
        await CreateFile<T>(fileType, fileName, objArray, preserveTypeInfo);
    }

    public static async UniTask<bool> WriteFile<T>(FileType fileType, T obj, string fileName = "", bool overwrite = true)
    {
        if (!isInit)
            Init();

        if (string.IsNullOrEmpty(fileName))
            fileName = $"{typeof(T).Name}";

        if(fileType == FileType.Log)
            fileName = $"{fileName}.txt";

        string path = filePath[(int)fileType];
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        using (FileStream fileStream = new FileStream($"{path}{fileName}", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            try
            {
                //Prepare data
                var settings = new JsonSerializerSettings();
                // This tells your serializer that multiple references are okay.
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                string json = JsonConvert.SerializeObject(obj, settings);
                //Debug.Log($"Save: {json}");
                byte[] datas;
                if (overwrite)
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                    if (fileType.Equals(FileType.Log))
                        datas = Encoding.UTF8.GetBytes($"{json}{Environment.NewLine}");
                    else
                        datas = await EncrpytStringToBytes(json, rijndaelManaged.Key, rijndaelManaged.IV);
                }
                else
                {
                    fileStream.Seek(0, SeekOrigin.End);
                    if (fileType.Equals(FileType.Log))
                        datas = Encoding.UTF8.GetBytes($"{json}{Environment.NewLine}");
                    else
                        datas = await EncrpytStringToBytes(json, rijndaelManaged.Key, rijndaelManaged.IV);
                }

                //Write
                await fileStream.WriteAsync(datas, 0, datas.Length);
                //await fileStream.FlushAsync();
            }
            catch (SerializationException exception)
            {
                Debug.Log("Save failed. Error: " + exception.Message);
                return false;
            }
            finally
            {
                fileStream.Close();
                onFinishSaveObj?.Invoke();
            }
            return true;
        }
    }

    public static async UniTask<bool> WriteFile<T>(FileType fileType, T[] objs, string fileName = "", bool overwrite = true)
    {
        if (!isInit)
            Init();

        if (string.IsNullOrEmpty(fileName))
            fileName = $"{typeof(T).Name}";

        string path = filePath[(int)fileType];
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        using (FileStream fileStream = new FileStream($"{path}{fileName}", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
        {
            try
            {
                //Prepare data
                var settings = new JsonSerializerSettings();
                // This tells your serializer that multiple references are okay.
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                string json = JsonConvert.SerializeObject(objs, settings);
                //Debug.Log($"Save: {json}");
                byte[] datas;
                if (overwrite)
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                    if (fileType.Equals(FileType.Log))
                        datas = Encoding.UTF8.GetBytes(json);
                    else
                        datas = await EncrpytStringToBytes(json, rijndaelManaged.Key, rijndaelManaged.IV);
                }
                else
                {
                    fileStream.Seek(0, SeekOrigin.End);
                    if (fileType.Equals(FileType.Log))
                        datas = Encoding.UTF8.GetBytes(json);
                    else
                        datas = await EncrpytStringToBytes(json, rijndaelManaged.Key, rijndaelManaged.IV);
                }

                //Write
                await fileStream.WriteAsync(datas, 0, datas.Length);
                //await fileStream.FlushAsync();
            }
            catch (SerializationException exception)
            {
                Debug.Log("Save failed. Error: " + exception.Message);
                return false;
            }
            finally
            {
                fileStream.Close();
                onFinishSaveObj?.Invoke();
            }
            return true;
        }
    }

    private static async UniTask<bool> CreateFile<T>(FileType fileType, string fileName, T[] objs, bool preserveTypeInfo = false)
    {
        if (!isInit)
            Init();

        if (string.IsNullOrEmpty(fileName))
            fileName = $"{typeof(T).Name}";

        string path = filePath[(int)fileType];
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        using (FileStream fileStream = new FileStream($"{path}{fileName}", FileMode.Create))
        {
            try
            {
                var settings = CreateJsonSettings(preserveTypeInfo);
                string json = JsonConvert.SerializeObject(objs, settings);
                Debug.Log($"Save: {json}");
                byte[] datas;
                if (fileType.Equals(FileType.Log))
                    datas = Encoding.UTF8.GetBytes(json);
                else
                    datas = await EncrpytStringToBytes(json, rijndaelManaged.Key, rijndaelManaged.IV);

                await fileStream.WriteAsync(datas, 0, datas.Length);
            }
            catch (SerializationException exception)
            {
                Debug.Log("Save failed. Error: " + exception.Message);
                return false;
            }
            finally
            {
                fileStream.Close();
                onFinishSaveObj?.Invoke();
            }
            return true;
        }
    }

    public static JsonSerializerSettings CreateJsonSettings(bool preserveTypeInfo = false)
    {
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        if (preserveTypeInfo)
            settings.TypeNameHandling = TypeNameHandling.Auto;
        return settings;
    }

    public static async UniTask<List<T>> LoadFile<T>(FileType fileType = FileType.Save, string key = "", bool preserveTypeInfo = false)
    {
        if (!isInit)
            Init();

        string path = $"{filePath[(int)fileType]}{typeof(T).Name}";

        if (!string.IsNullOrEmpty(key))
            path = $"{filePath[(int)fileType]}{key}";

        if (!File.Exists(path))
        {
            Debug.LogWarning($"FIOSystem Load: the path is not exists {path}");
            return new List<T>();
        }

        List<T> returnValue = new List<T>();
        try
        {
            byte[] rawMeta = File.ReadAllBytes(path);
            if (rawMeta == null || rawMeta.Length <= 0)
                return new List<T>();
            string meta = await DecryptStringFromBytes(rawMeta, rijndaelManaged.Key, rijndaelManaged.IV);
            Debug.Log($"meta: {meta}");
            returnValue = JsonConvert.DeserializeObject<List<T>>(meta, CreateJsonSettings(preserveTypeInfo));
        }
        catch (SerializationException exception)
        {
            if (exception != null)
                Debug.Log("SaveFile System Exception Catach!!\n" +
                    "Exception Source: " + exception.Source + "\n" +
                    "Exception Message: " + exception.Message);

            return returnValue;
        }
        finally
        {

        }

        return returnValue;
    }

    /// <summary>
    /// Escapes a CSV field by wrapping it in quotes if it contains commas, quotes, or newlines.
    /// Doubles any quotes within the field for proper CSV escaping.
    /// </summary>
    private static string EscapeCSVField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return field;
        
        // Check if field needs to be quoted (contains comma, quote, or newline)
        bool needsQuotes = field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r");
        
        if (needsQuotes)
        {
            // Escape any quotes by doubling them
            field = field.Replace("\"", "\"\"");
            // Wrap in quotes
            return $"\"{field}\"";
        }
        
        return field;
    }

    public static async UniTask<bool> SaveCSV<T>(string fileName, T[] objs)
    {
        if (!isInit)
            Init();

        string rawData = "";
        PropertyInfo[] propertyInfos = typeof(T).GetProperties();
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
            if (rawData.Length != 0)
                rawData += ",";
            rawData += EscapeCSVField(propertyInfo.Name);
        }
        rawData += "\n";
        foreach (T obj in objs)
        {
            bool isFirst = true;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (isFirst)
                    isFirst = false;
                else
                    rawData += ",";

                string value = propertyInfo.GetValue(obj)?.ToString() ?? "";
                rawData += EscapeCSVField(value);
            }
            rawData += "\n";
        }

        string path = filePath[0] + "/LocalConfig/";        
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        using (FileStream fileStream = new FileStream(path + fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
        {
            try
            {
                byte[] datas = await EncrpytStringToBytes(rawData, rijndaelManaged.Key, rijndaelManaged.IV);
                await fileStream.WriteAsync(datas, 0, datas.Length);
            }
            catch (SerializationException exception)
            {
                Debug.Log("SaveFile failed. Error: " + exception.Message);
                return false;
            }
            finally
            {
                fileStream.Close();
                onFinishSaveObj?.Invoke();
            }
            return true;
        }
    }

    public static async UniTask<bool> SaveCSV(string fileName, string csvData)
    {
        if (!isInit)
            Init();

        string path = filePath[(int)FileType.Config] + "/LocalConfig/";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        using (FileStream fileStream = new FileStream(path + fileName, FileMode.Create, FileAccess.ReadWrite))
        {
            try
            {
                byte[] datas = await EncrpytStringToBytes(csvData, rijndaelManaged.Key, rijndaelManaged.IV);
                await fileStream.WriteAsync(datas, 0, datas.Length);
            }
            catch (SerializationException exception)
            {
                Debug.Log("SaveFile failed. Error: " + exception.Message);
                return false;
            }
            finally
            {
                Debug.Log("Successfully saving encrpytion csv");
                fileStream.Close();
                onFinishSaveObj?.Invoke();
            }
            return true;
        }
    }

    /// <summary>
    /// Parses a CSV line properly handling quoted fields that may contain commas, quotes, and newlines.
    /// Handles stringified JSON and other comma-containing data.
    /// </summary>
    private static string[] ParseCSVLine(string line)
    {
        List<string> fields = new List<string>();
        bool inQuotes = false;
        StringBuilder currentField = new StringBuilder();
        
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            
            if (c == '"')
            {
                // Check if this is an escaped quote (two consecutive quotes)
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++; // Skip the next quote
                }
                else
                {
                    // Toggle the inQuotes flag
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                // End of field
                fields.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }
        
        // Add the last field
        fields.Add(currentField.ToString());
        
        return fields.ToArray();
    }

    public static async UniTask<List<T>> LoadEncryptedModel<T>()
    {
        if (!isInit)
            Init();

        List<T> result = new List<T>();
        string path = $"{filePath[(int)FileType.Config]}/LocalConfig/{typeof(T).Name}";
                
        string meta = "";
        if (!File.Exists(path))
        {
            Debug.LogWarning($"LoadEncryptedCSV: the path is not exists {path}");
            return new List<T>();
        }

        try
        {
            byte[] rawMeta = File.ReadAllBytes(path);
            if (rawMeta == null || rawMeta.Length <= 0)
                return new List<T>();
            meta = await DecryptStringFromBytes(rawMeta, rijndaelManaged.Key, rijndaelManaged.IV);
            //Debug.Log($"meta: {meta}");
        }
        catch (SerializationException exception)
        {
            if (exception != null)
                Debug.Log("SaveFile System Exception Catach!!\n" +
                    "Exception Source: " + exception.Source + "\n" +
                    "Exception Message: " + exception.Message);

            return null;
        }

        List<string> fieldNameList = new List<string>();
        string[] split = meta.Split('\n');
        Dictionary<string, PropertyInfo> propertyDict = null;
        foreach (string line in split)
        {
            try
            {
                if (line == null || line.Length == 0 || line == "")
                    continue;

                var values = ParseCSVLine(line);

                if (fieldNameList.Count == 0)
                    fieldNameList.AddRange(values);
                else
                {   
                    T newObj = (T)Activator.CreateInstance(typeof(T), new object[] { values[0] });

                    if (propertyDict == null)
                    {
                        propertyDict = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

                        var properties = typeof(T).GetProperties(BindingFlags.Public |
                                                                BindingFlags.Instance |
                                                                BindingFlags.FlattenHierarchy);

                        foreach (var prop in properties)
                        {
                            propertyDict[prop.Name] = prop;
                        }
                    }

                    for (int i = 1; i < fieldNameList.Count; i++)
                    {
                        if (propertyDict.TryGetValue(fieldNameList[i], out PropertyInfo propertyInfo))
                        {
                            propertyInfo.SetValue(newObj, Convert.ChangeType(values[i], propertyInfo.PropertyType));
                        }
                        else
                        {
                            Debug.LogError($"LoadEncryptedCSV: field not found[{fieldNameList[i]}] in {typeof(T).Name} or its base classes");
                            return result;
                        }
                    }
                    result.Add(newObj);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[{typeof(T)}]: {e}");
            }
        }

        /*
        foreach (string line in split)
        {
            try
            {
                //Debug.Log(line);
                if (line == null || line.Length == 0 || line == "")
                    continue;

                //Get raw data
                var values = line.Split(',');

                //if the first line
                if (fieldNameList.Count == 0)
                    fieldNameList.AddRange(values);
                else
                {
                    //CSV ds format must be id be the first column
                    T newObj = (T)Activator.CreateInstance(typeof(T), new object[] { values[0] });
                    for (int i = 1; i < fieldNameList.Count; i++)
                    {
                        PropertyInfo propertyInfo = newObj.GetType().GetProperty(fieldNameList[i]);
                        if (propertyInfo == null)
                        {
                            Debug.LogError($"LoadEncryptedCSV: field not found[{fieldNameList[i]}] in {typeof(T).Name}");
                            return result;
                        }
                        propertyInfo.SetValue(newObj, Convert.ChangeType(values[i], propertyInfo.PropertyType));
                    }
                    result.Add(newObj);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[{typeof(T)}]: {e}");
            }
        }
        */

        return result;
    }

    public static string LoadCSV(string path)
    {
        string result = "";
        
        Regex extensionRX = new Regex(@"\.csv$");
        if (path == string.Empty || !extensionRX.IsMatch(path))
        {
            Debug.LogError($"LoadCSV: Invalid Args");
            return result;
        }
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using (StreamReader streamReader = new StreamReader(fileStream))
        {
            while (!streamReader.EndOfStream)
            {
                result += streamReader.ReadLine();
                result += "\n";
            }
        }

        return result;
    }

    public static bool DeleteFile(FileType fileType,  string fileName)
    {
        try
        {
            string path = filePath[(int)fileType] + fileName;
            File.Delete(path);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    /*
    public static async UniTask<List<T>> LoadAllFile<T>(string directoryPath = "")
    {
        if (!isInit)
            Init();

        string[] filesName = GetAllFileInDirectory(directoryPath);
        if (filesName == null)
        {
            return null;
        }
        List<T> returnValue = new List<T>();
        foreach (string name in filesName)
        {
            string[] fileNameSplit = name.Split('.');
            if (fileNameSplit == null || fileNameSplit.Length == 0)
                continue;

            List<T> objs = await LoadFile<T>(fileNameSplit[0]);
            if (objs != null)
                returnValue.AddRange(objs);
        }
        return returnValue;
    }
    */

    private static async UniTask<byte[]> EncrpytStringToBytes(string data, byte[] key, byte[] iv)
    {
        // Check arguments.
        if (data == null || data.Length <= 0)
            throw new ArgumentNullException("data");
        if (key == null || key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (iv == null || iv.Length <= 0)
            throw new ArgumentNullException("IV");
        byte[] encrypted;
        // Create an RijndaelManaged object
        // with the specified key and IV.
        using (RijndaelManaged rijAlg = new RijndaelManaged())
        {
            rijAlg.Key = key;
            rijAlg.IV = iv;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {

                        //Write all data to the stream.
                        await swEncrypt.WriteAsync(data);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return encrypted;
    }

    private static async UniTask<string> DecryptStringFromBytes(byte[] meta, byte[] key, byte[] iv)
    {
        // Check arguments.
        if (meta == null || meta.Length <= 0)
        {
            //throw new ArgumentNullException("cipherText");
            return string.Empty;
        }
        if (key == null || key.Length <= 0)
        {
            //throw new ArgumentNullException("Key");
            return string.Empty;
        }
        if (iv == null || iv.Length <= 0)
        {
            //throw new ArgumentNullException("IV");
            return string.Empty;
        }

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;

        // Create an RijndaelManaged object
        // with the specified key and IV.
        using (RijndaelManaged rijAlg = new RijndaelManaged())
        {
            rijAlg.Key = key;
            rijAlg.IV = iv;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(meta))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = await srDecrypt.ReadToEndAsync();
                    }
                }
            }
        }

        return plaintext;
    }

    public static async UniTask<Int64> PredictObjectSaveSize(object[] objs)
    {
        if (!isInit)
            Init();

        Int64 fileSize = 0;

        try
        {
            var settings = new JsonSerializerSettings();
            // This tells your serializer that multiple references are okay.
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string json = JsonConvert.SerializeObject(objs, settings);
            Debug.Log($"SaveFile: {json}");
            byte[] datas = await EncrpytStringToBytes(json, rijndaelManaged.Key, rijndaelManaged.IV);
            fileSize = datas.LongLength;
        }
        catch (SerializationException exception)
        {
            Debug.Log("SaveFile failed. Error: " + exception.Message);
            return fileSize;
        }

        return fileSize;
    }

    public static async UniTask<byte[]> ToEncrpytedBinary<T>(T[] objs)
    {
        // Dynamic create a class to contain list of objs
        // Reason: easily extract data in 1 file(which i wanted to store a save in 1 file)
        Dictionary<string, Type> fields = new Dictionary<string, Type>();
        fields.Add(typeof(T).Name, typeof(T[]));
        var objType = DynamicTypeBuilder.CreateType(typeof(T).Name, fields);
        var saveObj = Activator.CreateInstance(objType);
        FieldInfo fieldInfo = objType.GetField(typeof(T).Name);
        fieldInfo.SetValue(saveObj, objs);
        // Using json to store objs
        var settings = new JsonSerializerSettings();
        // This tells your serializer that multiple references are okay.
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        string json = JsonConvert.SerializeObject(saveObj, settings);
        Debug.Log($"ToEncrpytedBinary: {json}");
        byte[] bytes = await EncrpytStringToBytes(json, rijndaelManaged.Key, rijndaelManaged.IV);
        return bytes;
    }

    public static string SizeSuffix(Int64 value, int decimalPlaces = 1)
    {
        if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
        if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
        if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

        // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
        int mag = (int)Math.Log(value, 1024);

        // 1L << (mag * 10) == 2 ^ (10 * mag) 
        // [i.e. the number of bytes in the unit corresponding to mag]
        decimal adjustedSize = (decimal)value / (1L << (mag * 10));

        // make adjustment when the value is large enough that
        // it would round up to 1000 or more
        if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
        {
            mag += 1;
            adjustedSize /= 1024;
        }

        return string.Format("{0:n" + decimalPlaces + "} {1}",
            adjustedSize,
            SizeSuffixes[mag]);
    }
}
