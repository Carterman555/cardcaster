using System;
using UnityEngine;

public class ES3EncryptionMigration : MonoBehaviour {
    private static string oldFileName = "SaveFile.es3";
    private static string newFileName = "EncryptedSaveFile.es3";
    private static string encryptionPassword = "BananasAreBest";

    [Header("Debug")]
    public bool debugMode = true;

    public static ES3Settings GetES3Settings() {
        if (ES3.FileExists(oldFileName)) {
            ES3Settings unencryptedSettings = new ES3Settings {
                location = ES3.Location.File,
                path = oldFileName
            };
            unencryptedSettings.encryptionType = ES3.EncryptionType.None;

            return unencryptedSettings;
        }
        else {
            ES3Settings encryptedSettings = new ES3Settings {
                encryptionPassword = encryptionPassword,
                location = ES3.Location.File,
                path = newFileName
            };
            encryptedSettings.encryptionType = ES3.EncryptionType.AES;

            return encryptedSettings;
        }
    }

    private void Start() {
        MigrateToEncryption();
    }

    public void MigrateToEncryption() {
        // Check if old unencrypted file exists
        if (ES3.FileExists(oldFileName)) {
            if (debugMode)
                Debug.Log($"Found old unencrypted file: {oldFileName}. Starting migration...");

            // Get all keys from the old unencrypted file
            string[] keys = ES3.GetKeys(oldFileName);

            if (keys.Length == 0) {
                if (debugMode)
                    Debug.Log("Old file is empty, deleting it.");
                ES3.DeleteFile(oldFileName);
                return;
            }

            ES3Settings encryptedSettings = new ES3Settings {
                encryptionPassword = encryptionPassword,
                location = ES3.Location.File,
                path = newFileName
            };
            encryptedSettings.encryptionType = ES3.EncryptionType.AES;

            // Migrate each key-value pair
            foreach (string key in keys) {
                object value = ES3.Load<object>(key, oldFileName);
                ES3.Save(key, value, encryptedSettings);

                if (debugMode)
                    Debug.Log($"Migrated key: {key}");
            }

            // Delete the old unencrypted file
            ES3.DeleteFile(oldFileName);

            if (debugMode)
                Debug.Log($"Migration complete! Data moved from {oldFileName} to {newFileName} with encryption.");
        }
        else {
            if (debugMode)
                Debug.Log($"No old file found ({oldFileName}). No migration needed.");
        }
    }


}