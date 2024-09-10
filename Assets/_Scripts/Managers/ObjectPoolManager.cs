using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ObjectPoolManager {
    public static List<PooledObjectInfo> ObjectPoolList = new List<PooledObjectInfo>();

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

        // Check if there are any iactive objects in the pool
        GameObject spawnableObject = pool.InactiveObject.FirstOrDefault();

        if (spawnableObject == null) {
            // If there are no inactive objects, create a new one
            spawnableObject = Object.Instantiate(objectToSpawn, spawnPosition, spawnRotation, parent);
        }
        else {
            // If there is an inactive object, reactive it
            spawnableObject.transform.position = spawnPosition;
            spawnableObject.transform.rotation = spawnRotation;
            spawnableObject.transform.SetParent(parent);
            pool.InactiveObject.Remove(spawnableObject);
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

        GameObject spawnedObject = Spawn(behaviourToSpawn.gameObject, Vector3.zero, Quaternion.identity, parent);
        spawnedObject.transform.localPosition = Vector3.zero;
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


        string goName = objectToReturn.name.Substring(0, objectToReturn.name.Length - 7);

        PooledObjectInfo pool = ObjectPoolList.Find(p => p.LookupString == goName);

        if (pool == null) {
            //Debug.LogWarning("Trying to release an object that is not pooled: " + objectToReturn.name + ". Destroying instead");
            Object.Destroy(objectToReturn);
        }
        else {
            objectToReturn.SetActive(false);
            pool.InactiveObject.Add(objectToReturn);
        }
    }
}

public class PooledObjectInfo {
    public string LookupString;
    public List<GameObject> InactiveObject = new();
}
