using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;

public class PlayerDash : MonoBehaviour
{
    UnitaskUtility fillTask = new UnitaskUtility();

    public Image[] dashImage;

    float dashRatio = 0; // ������ 0 ~ 1�� ����

    float DashRatio
    {
        get
        {
            return dashRatio;
        }
        set
        {
            dashRatio = value;

            if (dashRatio > 1) 
            {
                dashRatio = 1;
                return;
            }
        }
    }

    public int dashCount;
    public int maxDashCount = 3;

    private void Start()
    {
        GetComponent<Rigidbody2D>();
    }

    public void Dash()
    {
        if(CanUseDash() == true)
        {

            dashRatio -= 1 / maxDashCount;
            if(fillTask.NowRunning == false) FillDashRatio().Forget();
        }
    }

    bool CanUseDash()
    {
        return dashRatio - (1 / maxDashCount) >= 0;
    }

    void FillDashIcon(float ratio)
    {
        float perImage = 1 / maxDashCount; // ex) �̹����� 3���� ��� ���� 0.333
        bool fillComplete = false;

        // ratio�� 0.7�� ���, 2�� ä��� ������ 1�� 0.03333��ŭ�� ä���

        for (int i = 0; i < maxDashCount; i++)
        {
            if(fillComplete == true)
            {
                dashImage[i].fillAmount = 0;
                continue;
            }

            if(ratio > perImage * i)
            {
                if(dashImage[i].fillAmount != 1) dashImage[i].fillAmount = 1;
            }
            else if(ratio < perImage * i)
            {
                float lastFill = ratio - perImage * i;
                dashImage[i].fillAmount = lastFill * maxDashCount;
                fillComplete = true;
            }
        }
    }

    private async UniTaskVoid FillDashRatio()
    {
        fillTask.NowRunning = true;

        while (DashRatio < 1)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f), cancellationToken: fillTask.source.Token);
            DashRatio += 0.01f;

            FillDashIcon(DashRatio);
        }

        if(DashRatio != 1) DashRatio = 1;

        fillTask.NowRunning = false;
    }
    private void OnDestroy()
    {
        fillTask.WhenDestroy();
    }

    private void OnDisable()
    {
        fillTask.WhenDisable();
    }

    private void OnEnable()
    {
        fillTask.WhenEnable();
    }
}
