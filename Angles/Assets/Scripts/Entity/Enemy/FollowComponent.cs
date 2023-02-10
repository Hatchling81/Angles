using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;

public class FollowComponent : UnitaskUtility
{
    Entity entity;
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<Entity>();
        entity.fixedUpdateAction += MoveToPlayer;

        player = PlayManager.Instance.player;
        entity.PlayerMode = ActionMode.Follow;
    }

    public void WaitFollow()
    {
        if (NowRunning == true) return;
        WaitFollowTask().Forget();
    }

    public async UniTaskVoid WaitFollowTask()
    {
        NowRunning = true;
        entity.PlayerMode = ActionMode.Hit;

        entity.rigid.velocity = Vector2.zero;
        await UniTask.Delay(TimeSpan.FromSeconds(DatabaseManager.Instance.WaitTime), cancellationToken: source.Token);

        entity.PlayerMode = ActionMode.Follow;
        NowRunning = false;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            if(NowRunning == true)
            {
                Vector2 backDir = col.transform.position - transform.position;

                col.gameObject.GetComponent<FollowComponent>().WaitFollow();
                col.gameObject.GetComponent<BasicReflectComponent>().KnockBack(backDir * 3);
            }
        }
    }

    public void MoveToPlayer()
    {
        if (NowRunning == true)
        {
            return; // 루틴 돌아가는 동안, 사용 금지
        }

        //float distance = Vector2.Distance(player.transform.position, entity.transform.position);

        //if (distance < DatabaseManager.Instance.MinFollowDistance) { entity.PlayerMode = ActionMode.Idle; }
        //else { entity.PlayerMode = ActionMode.Follow; }

        //if (entity.PlayerMode == ActionMode.Idle || entity.PlayerMode == ActionMode.Hit) return;

        Vector2 dirVec = player.transform.position - entity.transform.position;
        Vector2 nextVec = dirVec.normalized * DatabaseManager.Instance.FollowSpeed * Time.fixedDeltaTime;
        entity.rigid.MovePosition(entity.rigid.position + nextVec);
    }
}
