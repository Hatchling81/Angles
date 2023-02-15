using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleComponent : MonoBehaviour
{
    List<Collision2D> entity = new List<Collision2D>();
    Player player;
    AttackComponent attackComponent;

    [SerializeField]
    List<BasicSkill> loadSkill = new List<BasicSkill>();
    // <-- null�̸� ��ų �����͸� �÷��̾�� �ҷ��ͼ� ���, �ƴϸ� ����Ǿ� �ִ� ��ü�� playskill �������ش�.
    // count ������ ���� 0�̸� ����Ʈ���� ����, 1 �̻��� ��� �ϳ��� ���ָ鼭 ���

    public EntityTag entityTag;

    private void Start()
    {
        player = GetComponent<Player>();
        attackComponent = GetComponent<AttackComponent>();
        player.collisionEnterAction += AddToList;
        player.collisionExitAction += RemoveToList;
        PlayManager.Instance.actionJoy.actionComponent.attackAction += PlayWhenAttackStart;
    }

    public void RemoveSkillFromLoad(BasicSkill skill)
    {
        loadSkill.Remove(skill);
    }

    public void AddSkillToLoad(BasicSkill skill)
    {
        loadSkill.Add(skill);
    }

    public void UseSkillInList(SkillUseType useType)
    {
        for (int i = 0; i < loadSkill.Count; i++)
        {
            if(loadSkill[i].SkillData.Type == useType)
            {
                loadSkill[i].PlaySkill(player.rigid.velocity.normalized, entity);
            }
        }
    }

    void AddToList(Collision2D col)
    {
        if (col.gameObject.CompareTag(entityTag.ToString()) != true) return;

        print(col);

        entity.Add(col);
        PlayWhenCollision();
    }

    void RemoveToList(Collision2D col) => entity.Remove(col);

    BasicSkill GetSkillUsingType(SkillName skillName)
    {
        BasicSkill skill = ObjectPooler.SpawnFromPool(skillName.ToString()).GetComponent<BasicSkill>();
        return skill;
    }

    bool NowContactEnemy()
    {
        return entity.Count > 0;
    }

    void UseSkill(SkillUseType skillUseType)
    {
        bool canUseSkill = player.SkillData.CanUseSkill(skillUseType);

        // ��ų�� ����� �� �ִ� ��� ��ų ���
        if (canUseSkill == true)
        {
            BasicSkill skill = GetSkillUsingType(player.SkillData.Name);
            if (skill == null) return;

            skill.Init(transform, this);

            AddSkillToLoad(skill); // �Ծ ����ϴ� ��ų�� �ְ�
            player.SkillData.ResetSkill(); // �÷��̾� ���� ��ų�� ����
        }
        else // ��ų�� ����� �� ���� ��� �⺻ ��ų�� ����ϰ� �Ѵ�.
        {
            BasicSkill normalSkill = GetSkillUsingType(player.NormalSkillData.Name);
            Debug.Log(normalSkill);

            if (normalSkill == null) return;

            normalSkill.Init(transform, this); // �⺻ ��ų�� �� �־ �� ��
            AddSkillToLoad(normalSkill); // �Ծ ����ϴ� ��ų�� �ְ�
        }

        UseSkillInList(skillUseType); // ����Ʈ�� �� ��� ������Ʈ�� ���Ǻη� ��������
    }
    
    void PlayWhenCollision()
    {
        if (NowContactEnemy() == false) return; // ���� �����ϰ� ���� ���� ��� ����
        if (player.PlayerMode != ActionMode.Attack) return;

        UseSkill(SkillUseType.Contact);

        attackComponent.QuickEndTask(); // ���� �������ֱ�
    }

    void PlayWhenAttackStart(Vector2 dir, ForceMode2D forceMode = ForceMode2D.Impulse)
    {
        PlayWhenCollision(); // �ѹ� üũ

        UseSkill(SkillUseType.Start);
    }

    public void PlayWhenGet()
    {
        UseSkill(SkillUseType.Get);
    }

    private void OnDisable()
    {
        if (player == null) return;
        player.collisionEnterAction -= AddToList;
        player.collisionExitAction -= RemoveToList;
    }
}
