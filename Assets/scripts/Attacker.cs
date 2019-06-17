using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float attackRange = 10.0f;
    public float bulletSpeed = 5.0f;
    public Transform attackRangeSphere;
    public float attackDelay = 2;
    public Transform target;
    public Transform attackPos;
    public bool showAttackRange = true;

    private Target targetScript;
    private float currAttackDelay = 0;

    // Start is called before the first frame update
    void Start()
    {
        targetScript = target.GetComponent<Target>();
    }

    // Update is called once per frame
    void Update()
    {
        currAttackDelay     -= Time.deltaTime;
        Vector3 aim         = target.position - transform.position;
        aim.y = 0;
        transform.forward = aim;
        if (currAttackDelay < 0 && aim.magnitude < attackRange)
        {
            Shot();
        }

        attackRangeSphere.localScale = new Vector3(attackRange, 1, attackRange);
        //attackRangeSphere

        if (showAttackRange && !attackRangeSphere.gameObject.activeSelf)
        {
            attackRangeSphere.gameObject.SetActive(true);
        }else if (!showAttackRange && attackRangeSphere.gameObject.activeSelf)
        {
            attackRangeSphere.gameObject.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = 0.25f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void Shot()
    {
        currAttackDelay = attackDelay;

        GameObject arrow = Instantiate(arrowPrefab);
        arrow.transform.position = attackPos.position;
        float time = 0;
        Vector3 hitPoint = GetHitPoint(targetScript.enemyAimPos, targetScript.lastSpeed, transform.position, bulletSpeed, out time);
        Vector3 aim = hitPoint - transform.position;
        aim.y = 0;

        //

        float antiGravity = - Physics.gravity.y * time / 2;
        float deltaY = (hitPoint.y - arrow.transform.position.y) / time;

        Vector3 arrowSpeed = aim.normalized * bulletSpeed;
        arrowSpeed.y = antiGravity + deltaY;



        arrow.GetComponent<Arrow>().Go(arrowSpeed, hitPoint);
    }

    //Keep arrow bulletSpeed > targetSpeed or hit position might not exist!
    Vector3 GetHitPoint(Vector3 targetPosition, Vector3 targetSpeed, Vector3 attackerPosition, float bulletSpeed, out float time)
    { 
        Vector3 q = targetPosition - attackerPosition;
        //Ignoring Y for now. Add gravity compensation later, for more simple formula and clean game design around it
        q.y = 0;
        targetSpeed.y = 0;

        //solving quadratic ecuation from t*t(Vx*Vx + Vy*Vy - S*S) + 2*t*(Vx*Qx)(Vy*Qy) + Qx*Qx + Qy*Qy = 0

        float a = Vector3.Dot(targetSpeed, targetSpeed) - (bulletSpeed * bulletSpeed); //Dot is basicly (targetSpeed.x * targetSpeed.x) + (targetSpeed.y * targetSpeed.y)
        float b = 2 * Vector3.Dot(targetSpeed, q); //Dot is basicly (targetSpeed.x * q.x) + (targetSpeed.y * q.y)
        float c = Vector3.Dot(q, q); //Dot is basicly (q.x * q.x) + (q.y * q.y)

        //Discriminant
        float D = Mathf.Sqrt((b * b) - 4 * a * c);

        float t1 = (-b + D) / (2 * a);
        float t2 = (-b - D) / (2 * a);

        Debug.Log("t1: " + t1 + " t2: " + t2);

        time = Mathf.Max(t1, t2);

        Vector3 ret = targetPosition + targetSpeed * time;
        return ret;
    }
}
