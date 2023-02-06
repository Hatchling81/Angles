using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;

public class DashUIComponent : UnitaskUtility
{
    public Image[] dashImage;

    private void Start()
    {
        WaitTIme = 0.1f;
    }

    public void ReFillDashRatio()
    {
        if (NowRunning != false) return;

        FillDashRatio().Forget();
    }
    void FillDashIcon(float ratio)
    {
        float perImage = 1 / DatabaseManager.Instance.MaxDashCount; // ex) �̹����� 3���� ��� ���� 0.333
        bool fillComplete = false;

        // ratio�� 0.7�� ���, 2�� ä��� ������ 1�� 0.03333��ŭ�� ä���

        for (int i = 1; i < DatabaseManager.Instance.MaxDashCount - 1; i++)
        {
            if (fillComplete == true)
            {
                dashImage[i].fillAmount = 0;
                continue;
            }


            // 0, 1, 2 --> 0.333333, 0.6666666, 0.999999
            if (ratio > perImage * i)
            {
                Debug.Log(perImage * i);
                Debug.Log(ratio);
                if (dashImage[i].fillAmount != 1) dashImage[i].fillAmount = 1;
            }
            else if (ratio < perImage * i)
            {
                Debug.Log(perImage * i);

                float lastFill = ratio - (perImage * i - 1); // �� ����Ʈ ������ ���� �� ����Ʈ�� ���� ä���� �� ���� ����
                dashImage[i].fillAmount = lastFill * DatabaseManager.Instance.MaxDashCount;
                fillComplete = true;
            }
        }
    }

    private async UniTaskVoid FillDashRatio()
    {
        NowRunning = true;

        while (DatabaseManager.Instance.DashRatio < 1)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(WaitTIme), cancellationToken: source.Token);
            DatabaseManager.Instance.DashRatio += 0.01f;

            FillDashIcon(DatabaseManager.Instance.DashRatio);
        }

        if (DatabaseManager.Instance.DashRatio != 1) DatabaseManager.Instance.DashRatio = 1;

        NowRunning = false;
    }
}
