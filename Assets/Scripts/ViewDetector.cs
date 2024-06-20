using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewDetector : MonoBehaviour
{
    [SerializeField] private GameObject target;
    public GameObject Target { get { return target; } }
    [SerializeField] private GameObject rangeTarget;
    public GameObject RangeTarget{ get { return rangeTarget; } }
    [SerializeField] private GameObject atkTarget;
    public GameObject AtkTarget { get { return atkTarget; } }

    [SerializeField] private float radiu;
    [SerializeField] private float angle;
    [SerializeField] private float rangeRadiu;
    public float rangeAngle;
    [SerializeField] private float atkRadiu;
    [SerializeField] private float atkAngle;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask obstaclemask;

    public void FindTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, radiu, layerMask);

        for(int i = 0; i < targets.Length; i++)
        {
            Vector3 findTarget = (targets[i].transform.position - transform.position).normalized;
            if(Vector3.Dot(transform.forward,findTarget) < Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad))
            {
                continue;
            }
            float findTargetRange = Vector3.Distance(transform.position, targets[i].transform.position);
            if(Physics.Raycast(transform.position, findTarget,findTargetRange,obstaclemask))
            {
                continue;
            }
            Debug.DrawRay(transform.position, findTarget * findTargetRange, Color.red);

            target = targets[i].gameObject;
            return;
        }
        target = null;
    }

    public void FindRangeTarget(float damage, float power)
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, rangeRadiu, layerMask);

        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 findTarget = (targets[i].transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, findTarget) < Mathf.Cos(rangeAngle * 0.5f * Mathf.Deg2Rad))
            {
                continue;
            }
            float findTargetRange = Vector3.Distance(transform.position, targets[i].transform.position);
            if (Physics.Raycast(transform.position, findTarget, findTargetRange, obstaclemask))
            {
                continue;
            }
            Debug.DrawRay(transform.position, findTarget * findTargetRange, Color.red);

            rangeTarget = targets[i].gameObject;
            if(rangeTarget != null)
            {
                rangeTarget.GetComponent<IInteractable>()?.TakeHit(damage);
                rangeTarget.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up) * power, ForceMode.Impulse);
            }
        }
        rangeTarget = null;
    }

    public void FindQSkillTarget(float damage, float power)
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, rangeRadiu, layerMask);

        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 findTarget = (targets[i].transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, findTarget) < Mathf.Cos(rangeAngle * 0.5f * Mathf.Deg2Rad))
            {
                continue;
            }
            float findTargetRange = Vector3.Distance(transform.position, targets[i].transform.position);
            if (Physics.Raycast(transform.position, findTarget, findTargetRange, obstaclemask))
            {
                continue;
            }
            Debug.DrawRay(transform.position, findTarget * findTargetRange, Color.red);

            rangeTarget = targets[i].gameObject;
            if (rangeTarget != null)
            {
                rangeTarget.GetComponent<IInteractable>()?.TakeHit(damage);
                rangeTarget.GetComponent<Rigidbody>().AddForce((transform.forward + transform.right) * power, ForceMode.Impulse);
            }
        }
        rangeTarget = null;
    }

    public void FindAttackTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, atkRadiu, layerMask);

        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 findTarget = (targets[i].transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, findTarget) < Mathf.Cos(atkAngle * 0.5f * Mathf.Deg2Rad))
            {
                continue;
            }
            float findTargetRange = Vector3.Distance(transform.position, targets[i].transform.position);
            if (Physics.Raycast(transform.position, findTarget, findTargetRange, obstaclemask))
            {
                continue;
            }
            Debug.DrawRay(transform.position, findTarget * findTargetRange, Color.red);

            atkTarget = targets[i].gameObject;
            return;
        }
        atkTarget = null;
    }

    public void FindPlayerTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, rangeRadiu, layerMask);

        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 findTarget = (targets[i].transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, findTarget) < Mathf.Cos(rangeAngle * 0.5f * Mathf.Deg2Rad))
            {
                continue;
            }
            float findTargetRange = Vector3.Distance(transform.position, targets[i].transform.position);
            if (Physics.Raycast(transform.position, findTarget, findTargetRange, obstaclemask))
            {
                continue;
            }
            Debug.DrawRay(transform.position, findTarget * findTargetRange, Color.red);

            rangeTarget = targets[i].gameObject;
            return;
        }
        rangeTarget = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radiu);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangeRadiu);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, atkRadiu);

        Vector3 lookDir = AngleToDir(transform.eulerAngles.y);
        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + angle * 0.5f);
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - angle * 0.5f);

        Debug.DrawRay(transform.position, lookDir * radiu, Color.green);
        Debug.DrawRay(transform.position, rightDir * radiu, Color.green);
        Debug.DrawRay(transform.position, leftDir * radiu, Color.green);

        Vector3 rangeRightDir = AngleToDir(transform.eulerAngles.y + rangeAngle * 0.5f);
        Vector3 rangeLeftDir = AngleToDir(transform.eulerAngles.y - rangeAngle * 0.5f);

        Debug.DrawRay(transform.position, lookDir * rangeRadiu, Color.green);
        Debug.DrawRay(transform.position, rangeRightDir * rangeRadiu, Color.green);
        Debug.DrawRay(transform.position, rangeLeftDir * rangeRadiu, Color.green);

        Vector3 atkRightDir = AngleToDir(transform.eulerAngles.y + atkAngle * 0.5f);
        Vector3 atkLeftDir = AngleToDir(transform.eulerAngles.y - atkAngle * 0.5f);

        Debug.DrawRay(transform.position, lookDir * atkRadiu, Color.red);
        Debug.DrawRay(transform.position, atkRightDir * atkRadiu, Color.red);
        Debug.DrawRay(transform.position, atkLeftDir * atkRadiu, Color.red);
    }

    private Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian));
    }
}
