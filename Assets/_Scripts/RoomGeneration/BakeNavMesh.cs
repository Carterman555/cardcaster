using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeNavMesh : MonoBehaviour {

    private NavMeshSurface navMeshSurface;

    private void Awake() {
        navMeshSurface = GetComponent<NavMeshSurface>();
        navMeshSurface.hideEditorLogs = true;
    }

    private void OnEnable() {
        RoomGenerator.OnCompleteGeneration += Bake;
    }
    private void OnDisable() {
        RoomGenerator.OnCompleteGeneration -= Bake;
    }

    public void Bake() {
        Physics2D.SyncTransforms();
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }

}
