using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;

public class AttackComponent : UnitaskUtility
{
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        player.fixedUpdateAction += SubUpdate;

        PlayManager.Instance.actionJoy.actionComponent.aniAction += ResetReadyAni;
        PlayManager.Instance.actionJoy.actionComponent.attackAction += PlayAttack;
    }
    void ResetReadyAni(bool nowReady) => player.Animator.SetBool("NowReady", nowReady);

    void SubUpdate()
    {
        if (player.PlayerMode != PlayerMode.Attack) return;

        if (PlayManager.Instance.moveJoy.moveInputComponent.NowCancelAttack() == true) // ���� ������ ����ؾ��� ���
        {
            CancelTask();
            CancelAttack();
        }
    }

    public void PlayAttack(Vector2 attackDir)
    {
        if (player.PlayerMode != PlayerMode.Idle) return;

        player.rigid.velocity = Vector2.zero;

        PlayManager.Instance.moveJoy.moveInputComponent.SetLoadVec(); // ���� ���͸� �����صд�.

        ResetReadyAni(false);
        player.Animator.SetBool("NowAttack", true);

        player.rigid.AddForce(-attackDir * DatabaseManager.Instance.AttackThrust, ForceMode2D.Impulse);
        player.PlayerMode = PlayerMode.Attack;
        WaitAttackEndTask().Forget();
    }

    public async UniTaskVoid WaitAttackEndTask()
    {
        NowRunning = true;

        await UniTask.Delay(TimeSpan.FromSeconds(DatabaseManager.Instance.AttackTime), cancellationToken: source.Token);
        CancelAttack();
        NowRunning = false;
    }

    public void CancelAttack()
    {
        player.PlayerMode = PlayerMode.Idle;
        player.Animator.SetBool("NowAttack", false);
    }

    protected override  void OnDisable()
    {
        base.OnDisable();

        player.fixedUpdateAction -= SubUpdate;
        PlayManager.Instance.actionJoy.actionComponent.aniAction -= ResetReadyAni;
        PlayManager.Instance.actionJoy.actionComponent.attackAction -= PlayAttack;
    }
}
