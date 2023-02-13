using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBombSkill : BasicSkill
{
    protected override void OnEnable()
    {
        base.OnEnable();
        SkillData.SkillUseCount = 2; // ���Ƚ�� 2��
    }

    public override void PlaySkill(Vector2 dir, List<Collision2D> entity)
    {
        print(entity.Count);
        for (int i = 0; i < entity.Count; i++)
        {
            print(entity[i]);
        }

        GameObject effectGo = GetEffectUsingName("StickyBombEffect", transform.position, transform.rotation);
        effectGo.GetComponent<ExplosionEffect>().SetExplodePos(entity[0].transform);

        base.PlaySkill(dir, entity);
    }
}
