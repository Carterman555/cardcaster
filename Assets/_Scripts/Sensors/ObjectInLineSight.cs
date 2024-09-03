using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInLineSight {

    private Transform target;
    private LayerMask layerMask;
    private float castRadius;

    public ObjectInLineSight(Transform target, LayerMask layerMask, float castRadius) {
        this.target = target;
        this.layerMask = layerMask;
        this.castRadius = castRadius;
    }

    public bool InSight(Vector2 origin) {
        Vector2 toTarget = (Vector2)target.position - origin;
        float distance = 100f;
        RaycastHit2D hit = Physics2D.CircleCast(origin, castRadius, toTarget, distance, layerMask);
        return hit.transform == target;
    }
}
