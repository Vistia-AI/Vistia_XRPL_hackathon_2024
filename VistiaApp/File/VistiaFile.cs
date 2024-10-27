using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class VistiaFile

{
    private string GetFilePath(string filePath, string fileName) => Path.Combine(Application.persistentDataPath, filePath, fileName + ".vistia");

    // Save file with or without password
    public void Save<T>(string filePath, string fileName, T data, MetadataFile metadata, string password = "defaultPassword")
    {
        bool passwordProtected = !string.IsNullOrEmpty(password);

        using (MemoryStream ms = new MemoryStream())
        {
            // Serialize metadata as JSON
            string metadataJson = JsonConvert.SerializeObject(metadata);
            byte[] metadataBytes = Encoding.UTF8.GetBytes(metadataJson);

            // Write metadata length and metadata bytes
            ms.Write(BitConverter.GetBytes(metadataBytes.Length), 0, 4);
            ms.Write(metadataBytes, 0, metadataBytes.Length);

            // Encrypt the file with the user's password
            byte[] salt = GenerateSalt();
            byte[] key = DeriveKey(password, salt);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();
                byte[] iv = aes.IV;

                ms.Write(salt, 0, salt.Length); // Store the salt
                ms.Write(iv, 0, iv.Length); // Store the IV

                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] dataBytes = SerializeData(data);
                    cs.Write(dataBytes, 0, dataBytes.Length);
                }
            }

            string fullPath = GetFilePath(filePath, fileName);

            // Create the directory if it does not exist
            string directoryPath = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Save the final file
            File.WriteAllBytes(fullPath, ms.ToArray());
            Debug.Log("File saved successfully!");
        }
    }

    // Load file and determine if password is required
    public (T, MetadataFile) Load<T>(string filePath, string password = "defaultPassword")
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found!");
            return default;
        }

        byte[] fileBytes = File.ReadAllBytes(filePath);
        using (MemoryStream ms = new MemoryStream(fileBytes))
        {

            // Read metadata length and metadata bytes
            byte[] metadataLengthBytes = new byte[4];
            ms.Read(metadataLengthBytes, 0, metadataLengthBytes.Length);
            int metadataLength = BitConverter.ToInt32(metadataLengthBytes, 0);

            byte[] metadataBytes = new byte[metadataLength];
            ms.Read(metadataBytes, 0, metadataBytes.Length);

            MetadataFile metadata = JsonConvert.DeserializeObject<MetadataFile>(Encoding.UTF8.GetString(metadataBytes));

            // File is encrypted, prompt for password
            if (string.IsNullOrEmpty(password))
            {
                Debug.LogError("Password is required for this file.");
                return default;
            }

            // Extract salt and IV
            byte[] salt = new byte[16];
            byte[] iv = new byte[16];
            ms.Read(salt, 0, salt.Length);
            ms.Read(iv, 0, iv.Length);

            // Derive the key from the password
            byte[] key = DeriveKey(password, salt);

            // Decrypt the file content
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    try
                    {
                        T data = DeserializeData<T>(cs);
                        return (data, metadata);
                    }
                    catch (CryptographicException e)
                    {
                        Debug.LogError("Invalid password!: " + e.Message);
                        return default;
                    }
                }
            }
        }
    }

    public bool IsFileExists(string filePath, string fileName)
    {
        return File.Exists(GetFilePath(filePath, fileName));
    }

    // Load file and determine if password is required
    public (T, MetadataFile) Load<T>(string filePath, string fileName, string password = "defaultPassword")
    {
        filePath = GetFilePath(filePath, fileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found!");
            return default;
        }

        byte[] fileBytes = File.ReadAllBytes(filePath);
        using (MemoryStream ms = new MemoryStream(fileBytes))
        {

            // Read metadata length and metadata bytes
            byte[] metadataLengthBytes = new byte[4];
            ms.Read(metadataLengthBytes, 0, metadataLengthBytes.Length);
            int metadataLength = BitConverter.ToInt32(metadataLengthBytes, 0);

            byte[] metadataBytes = new byte[metadataLength];
            ms.Read(metadataBytes, 0, metadataBytes.Length);

            MetadataFile metadata = JsonConvert.DeserializeObject<MetadataFile>(Encoding.UTF8.GetString(metadataBytes));

            // File is encrypted, prompt for password
            if (string.IsNullOrEmpty(password))
            {
                Debug.LogError("Password is required for this file.");
                return default;
            }

            // Extract salt and IV
            byte[] salt = new byte[16];
            byte[] iv = new byte[16];
            ms.Read(salt, 0, salt.Length);
            ms.Read(iv, 0, iv.Length);

            // Derive the key from the password
            byte[] key = DeriveKey(password, salt);

            // Decrypt the file content
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    try
                    {
                        T data = DeserializeData<T>(cs);
                        return (data, metadata);
                    }
                    catch (CryptographicException e)
                    {
                        Debug.LogError("Invalid password!: " + e.Message);
                        return default;
                    }
                }
            }
        }
    }

    public MetadataFile LoadMetadata<T>(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found!");
            return default;
        }

        MetadataFile metadata;
        byte[] fileBytes = File.ReadAllBytes(filePath);
        using (MemoryStream ms = new MemoryStream(fileBytes))
        {

            // Read metadata length and metadata bytes
            byte[] metadataLengthBytes = new byte[4];
            ms.Read(metadataLengthBytes, 0, metadataLengthBytes.Length);
            int metadataLength = BitConverter.ToInt32(metadataLengthBytes, 0);

            byte[] metadataBytes = new byte[metadataLength];
            ms.Read(metadataBytes, 0, metadataBytes.Length);

            metadata = JsonConvert.DeserializeObject<MetadataFile>(Encoding.UTF8.GetString(metadataBytes));
        }

        return metadata;
    }

    // Helper method to serialize any data type to a byte array
    private byte[] SerializeData<T>(T data)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, data);
            return ms.ToArray();
        }
    }

    // Helper method to deserialize data of generic type T
    private T DeserializeData<T>(Stream stream)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        return (T)formatter.Deserialize(stream);
    }

    // Generate random salt
    private byte[] GenerateSalt(int size = 16)
    {
        var salt = new byte[size];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    // Derive a key using PBKDF2
    private byte[] DeriveKey(string password, byte[] salt, int keySize = 32, int iterations = 1000)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
        {
            return pbkdf2.GetBytes(keySize);
        }
    }
}
