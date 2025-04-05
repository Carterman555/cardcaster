using NavMeshPlus.Components;
using System.Collections;
using UnityEngine;

public class BakeNavMesh : MonoBehaviour {

    private NavMeshSurface navMeshSurface;

    private void Awake() {
        navMeshSurface = GetComponent<NavMeshSurface>();
        navMeshSurface.hideEditorLogs = true;
    }

    private void OnEnable() {
        RoomGenerator.OnCompleteGeneration += Bake;
        Tutorial.OnTutorialRoomStart += Bake;
    }
    private void OnDisable() {
        RoomGenerator.OnCompleteGeneration -= Bake;
        Tutorial.OnTutorialRoomStart -= Bake;
    }

    public void Bake() {
        StartCoroutine(BakeCor());
    }

    private IEnumerator BakeCor() {
        //... wait for RandomlyActive class to set which objects are active, so they don't block movement when inactive
        yield return null; 

        Physics2D.SyncTransforms();
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }

}
