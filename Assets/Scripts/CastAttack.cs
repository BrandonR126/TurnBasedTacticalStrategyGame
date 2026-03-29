using UnityEngine;


public enum CoverType { None, Half, Full }

public struct Rays {
    public bool HasLOS;
    public CoverType Cover;

    public bool hasValidRay;

    public float rayDistance;
}


public class CastAttack : MonoBehaviour
{
    [Header("Ray Start/End Points")]
    public float shooterEyeHeight = 1f;
    public float targetChestHeight = 0.8f;
    public float sideOffset = 0.5f;

    [Header("Layer Masks")]
    public LayerMask worldMask;
    public LayerMask coverMask;
    public LayerMask unitMask;

    public Rays[] Cast3Rays(Vector3 shooter, Vector3 target) {
        // Set y to 0 to ignore height differences in LOS calculation; helps with calculating hit chance when target is simulated through tile pos

        // NOTE: THIS WILL BREAK WITH MULTI-LEVEL MAPS, REWORK IF MULTI LEVEL MAPS ARE ADDED
        shooter.y = 0;
        target.y = 0;

        Vector3 rayDirection = (target - shooter).normalized;

        // Get ray start and end points
        Vector3 startPointCenter = shooter;
        startPointCenter.y = shooterEyeHeight;

        Vector3 startPointLeft = startPointCenter;
        Vector3 startPointRight = startPointCenter;

        if (Mathf.Abs(rayDirection.x) < Mathf.Abs(rayDirection.z)) {

            startPointLeft.x -= sideOffset;

            startPointRight.x += sideOffset;
        }
        else {
            startPointLeft.z -= sideOffset;

            startPointRight.z += sideOffset;

        }

        Vector3 targetPoint = target;
        targetPoint.y = targetChestHeight;

        Vector3[] startPoints = {startPointLeft, startPointCenter, startPointRight};

        Rays[] results = new Rays[3];

        for (int i = 0; i < startPoints.Length; i++) // find best ray
        {
            results[i] = Cast1Ray(startPoints[i], targetPoint);

            results[i].rayDistance = Vector3.Distance(startPoints[i], targetPoint);

        }

        return results;
    }

    private Rays Cast1Ray(Vector3 start, Vector3 end) { // check ray
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        Rays result = new Rays();
        result.HasLOS = false;
        result.Cover = CoverType.None;

        LayerMask hitMask = worldMask | coverMask | unitMask;

        RaycastHit[] hits = Physics.RaycastAll(start, direction, distance, hitMask);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits) {
            int layer = hit.collider.gameObject.layer;

            if (worldMask == (worldMask | (1 << layer))) {
                Debug.DrawLine(start, hit.point, Color.red, 5f);
                return result;
            }

            if(coverMask == (coverMask | (1 << layer))) {
                if(layer == LayerMask.NameToLayer("FullCover")) {
                    result.Cover = CoverType.Full;
                    Debug.DrawLine(start, hit.point, Color.red, 5f);
                    return result;
                }
                else if(layer == LayerMask.NameToLayer("HalfCover")) {
                    Debug.DrawLine(start, hit.point, Color.blue, 5f);
                    result.Cover = CoverType.Half;
                }
            }

            if(unitMask == (unitMask | (1 << layer))) {

                if(Vector3.Distance(end, hit.point) >=1) { // hit wrong unit
                    Debug.DrawLine(start, hit.point, Color.red, 5f);
                    return result;
                }

                result.HasLOS = true;
                Debug.DrawLine(start, hit.point, Color.green, 5f);
                return result;
            }
        }
        Debug.DrawLine(start, end, Color.white, 5f);
        return result;
    }
}
