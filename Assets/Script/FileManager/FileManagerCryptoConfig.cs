using UnityEngine;

/// <summary>
/// AES key/IV for FileManager. Create via Assets → Create → Null Template → File Manager Crypto Config
/// and place at Resources/FileManagerCryptoConfig.
/// </summary>
[CreateAssetMenu(fileName = "FileManagerCryptoConfig", menuName = "Null Template/File Manager Crypto Config")]
public class FileManagerCryptoConfig : ScriptableObject
{
    [Tooltip("32-char AES key (UTF8 bytes used as-is).")]
    public string aesKey = "CHANGE_ME_32_CHAR_AES_KEY_!!!!!!";

    [Tooltip("16-char AES IV (UTF8 bytes used as-is).")]
    public string aesIv = "CHANGE_ME_16_IV!";

    public bool IsPlaceholder
    {
        get
        {
            return string.IsNullOrEmpty(aesKey)
                || string.IsNullOrEmpty(aesIv)
                || aesKey.StartsWith("CHANGE_ME")
                || aesIv.StartsWith("CHANGE_ME");
        }
    }
}
