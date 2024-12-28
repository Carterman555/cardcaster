using Mono.CSharp;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class ObjectPoolManager {
    public static List<PooledObjectInfo> ObjectPoolList = new List<PooledObjectInfo>();

    // debugging
    private static string nameToDebug = "";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init() {
        ObjectPoolList = new List<PooledObjectInfo>();
    }

    public static GameObject Spawn(this GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, Transform parent = null) {
        if (objectToSpawn == null) {
            Debug.LogError("objectToSpawn Is Null!");
            return null;
        }

        PooledObjectInfo pool = ObjectPoolList.Find(pool => pool.LookupString == objectToSpawn.name);

        // If the pool doesn't exist, create it
        if (pool == null) {
            pool = new PooledObjectInfo() {
                LookupString = objectToSpawn.name
            };
            ObjectPoolList.Add(pool);
        }

        // Check if there are any inactive objects in the pool
        GameObject spawnableObject = pool.InactiveObjects.FirstOrDefault();

        if (spawnableObject == null) {
            // If there are no inactive objects, create a new one
            spawnableObject = Object.Instantiate(objectToSpawn, spawnPosition, spawnRotation, parent);
        }
        else {
            // If there is an inactive object, reactive it
            spawnableObject.transform.position = spawnPosition;
            spawnableObject.transform.rotation = spawnRotation;
            spawnableObject.transform.SetParent(parent);
            pool.InactiveObjects.Remove(spawnableObject);
            spawnableObject.SetActive(true);
        }

        return spawnableObject;
    }

    public static GameObject Spawn(this GameObject objectToSpawn, Vector3 spawnPosition, Transform parent = null) {
        if (objectToSpawn == null) {
            Debug.LogError("behaviourToSpawn Is Null!");
            return null;
        }

        GameObject spawnedObject = Spawn(objectToSpawn, spawnPosition, Quaternion.identity, parent);
        return spawnedObject;
    }

    public static GameObject Spawn(this GameObject objectToSpawn, Transform parent = null) {
        if (objectToSpawn == null) {
            Debug.LogError("behaviourToSpawn Is Null!");
            return null;
        }

        GameObject spawnedObject = Spawn(objectToSpawn, Vector3.zero, Quaternion.identity, parent);
        spawnedObject.transform.localPosition = Vector3.zero;
        spawnedObject.transform.localRotation = Quaternion.identity;
        return spawnedObject;
    }

    public static T Spawn<T>(this T behaviourToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, Transform parent = null) where T : Component {
        if (behaviourToSpawn == null) {
            Debug.LogError("behaviourToSpawn Is Null!");
            return null;
        }

        GameObject spawnedObject = Spawn(behaviourToSpawn.gameObject, spawnPosition, spawnRotation, parent);
        return spawnedObject.GetComponent<T>();
    }

    public static T Spawn<T>(this T behaviourToSpawn, Vector3 spawnPosition, Transform parent = null) where T : Component {
        if (behaviourToSpawn == null) {
            Debug.LogError("behaviourToSpawn Is Null!");
            return null;
        }

        GameObject spawnedObject = Spawn(behaviourToSpawn.gameObject, spawnPosition, Quaternion.identity, parent);
        return spawnedObject.GetComponent<T>();
    }

    public static T Spawn<T>(this T behaviourToSpawn, Transform parent = null) where T : Component {
        if (behaviourToSpawn == null) {
            Debug.LogError("behaviourToSpawn Is Null!");
            return null;
        }

        GameObject spawnedObject = Spawn(behaviourToSpawn.gameObject, parent);
        return spawnedObject.GetComponent<T>();
    }

    public static void ReturnToPool(this GameObject objectToReturn) {

        if (objectToReturn == null) {
            Debug.LogError("objectToReturn Is Null!");
            return;
        }

        if (objectToReturn.name.Length < 8) {
            //Debug.LogWarning("Name" + objectToReturn.name + " is under 8 characters. Destroying instead");
            Object.Destroy(objectToReturn);
            return;
        }


        string goName = objectToReturn.name[..^7];
        PooledObjectInfo pool = ObjectPoolList.Find(p => p.LookupString == goName);

        if (pool == null) {
            //Debug.LogWarning("Trying to release an object that is not pooled: " + objectToReturn.name + ". Destroying instead");
            Object.Destroy(objectToReturn);
        }
        else {

            if (pool.InactiveObjects.Contains(objectToReturn)) {

                if (objectToReturn.activeSelf) {
                    objectToReturn.SetActive(false);
                    Debug.LogWarning($"Trying to return {objectToReturn.name} which was already returned! (Was active so set Inactive)");
                }
                else {
                    Debug.LogWarning($"Trying to return {objectToReturn.name} which was already returned!");
                }

                return;
            }

            // debug
            if (goName == nameToDebug) {
                Debug.Log($"returned {nameToDebug}");
            }

            pool.InactiveObjects.Add(objectToReturn);
            objectToReturn.SetActive(false);
        }
    }

    public static void TryReturnToPool(this GameObject objectToReturn) {
        if (!objectToReturn.IsReturned()) {
            objectToReturn.ReturnToPool();
        }
    }

    public static bool IsReturned(this GameObject objectToCheck) {

        // if the gameobject is not long enough it means it doesn't end in 'clone', so the object wasn't spawn in
        // so just return whether it's active
        if (objectToCheck.name.Length <= 7) {
            return !objectToCheck.activeSelf;
        }

        string goName = objectToCheck.name[..^7];
        PooledObjectInfo pool = ObjectPoolList.Find(p => p.LookupString == goName);

        //... if pool doesn't exist with this object
        if (pool == null) {
            return false;
        }

        bool inPool = pool.InactiveObjects.Contains(objectToCheck);
        return inPool;
    }
}

public class PooledObjectInfo {
    public string LookupString;
    public List<GameObject> InactiveObjects = new();
}
