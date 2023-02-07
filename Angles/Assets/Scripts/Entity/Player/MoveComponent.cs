using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveComponent : MonoBehaviour
{
    float angle = 0;
    Player player;
    Rigidbody2D rigid;

    Vector2 rotationVec = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        player.fixedUpdateAction += SubUpdate;

        rigid = GetComponent<Rigidbody2D>();
    }

    public void RotateUsingVelocity()
    {
        float tempAngle = angle;

        angle = Mathf.Atan2(rigid.velocity.normalized.y, rigid.velocity.normalized.x) * Mathf.Rad2Deg;
        if (angle == tempAngle) return;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void SubUpdate()
    {
        if (player.PlayerMode != PlayerMode.Idle) return;
        Move();
    }

    public void Move()
    {
        RotateUsingVelocity();
        bool nowReady = PlayManager.Instance.actionJoy.actionComponent.Mode == ActionMode.AttackReady;
        
        if (nowReady == true)
        {
            rigid.velocity = PlayManager.Instance.moveJoy.moveInputComponent.ReturnMoveVec().normalized * DatabaseManager.Instance.ReadySpeed;
        }
        else
        {
            rigid.velocity = PlayManager.Instance.moveJoy.moveInputComponent.ReturnMoveVec().normalized * DatabaseManager.Instance.MoveSpeed;
        }
    }

    private void OnDisable()
    {
        player.fixedUpdateAction -= SubUpdate;
    }
}
