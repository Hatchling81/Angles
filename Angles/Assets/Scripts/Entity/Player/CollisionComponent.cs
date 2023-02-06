using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionComponent : MonoBehaviour
{
    Player player;
    AttackComponent attackComponent;
    Rigidbody2D rigid;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        attackComponent = GetComponent<AttackComponent>();
        rigid = GetComponent<Rigidbody2D>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (player.PlayerMode != PlayerMode.Attack) return;

        if (collision.gameObject.CompareTag("Wall") == true)
        {
            WallColorChange colorChange = collision.gameObject.GetComponent<WallColorChange>();
            colorChange.ChangeTileColor(ReturnHitPosition(collision));

            ReflectPlayer(collision.contacts[0].normal);

            attackComponent.CancelTask();
            attackComponent.WaitAttackEndTask().Forget(); // ���� ���ߴ� �� ����
        }
    }

    Vector3 ReturnHitPosition(Collision2D collision)
    {
        Vector3 hitPosition = Vector3.zero;
        foreach (ContactPoint2D hit in collision.contacts)
        {
            hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
            hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
        }

        return hitPosition;
    }

    void ReflectPlayer(Vector2 hitPoint)
    {
        Vector2 velocity = rigid.velocity;
        var dir = Vector2.Reflect(velocity.normalized, hitPoint);
        rigid.velocity = dir * (DatabaseManager.Instance.AttackThrust / 2);
    }
}