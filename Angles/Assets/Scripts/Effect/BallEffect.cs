using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEffect : BasicEffect
{
    protected override void OnEnable()
    {
        PlayEffect();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        print(col.gameObject.name); // ����Ʈ�� ���� - ��� ���� ����
        ObjectPooler.ReturnToPool(gameObject, true);    // �� ��ü�� �ѹ���
        DisableObject();
    }


    protected override void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject, true);    // �� ��ü�� �ѹ��� 
        CancelInvoke();    // Monobehaviour�� Invoke�� �ִٸ�
    }
}
