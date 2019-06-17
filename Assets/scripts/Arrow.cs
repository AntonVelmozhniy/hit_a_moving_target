using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody body;
    public bool hit = false;
    public Transform target;
    public float m_life = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        target.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hit)
        {
            transform.forward = body.velocity;
        }

        if (hit)
        {
            m_life -= Time.deltaTime;
            if(m_life < 0)
            {
                Destroy(target.gameObject);
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!hit)
        {
            if (collision.gameObject.tag == "Target")
            {
                transform.parent = collision.collider.transform;
            }

            hit = true;
            Destroy(body);
            Destroy(GetComponent<BoxCollider>());
            //transform.scale 
        }
    }
    
    public void Go(Vector3 speed, Vector3 targetPos)
    {
        body.velocity = speed;
        target.position = targetPos;
    }
}
