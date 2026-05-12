using UnityEngine;

public class AttackState : EnemyFSMBase
{
    //ÉËº¦Ê±¼ä¼ä¸ô
    public AttackState(Enemy enemy) { this.enemy = enemy; }

    public override void enter(Enemy enemy)
    {
        Debug.Log("½øÈë¹¥»÷×´Ì¬");
        enemy.anim.SetBool("Is_Attacking",true);
    }

    public override EnemyFSMBase handleInput(PlayerInput input)
    {
        if (input == PlayerInput.PRESS_W) 
        {
            Debug.Log("¹¥»÷×´Ì¬ÇÐ»»ÖÁÒÆ¶¯×´Ì¬");
            return new MoveState(enemy);
        }
        return null;
    }

    public override void update()
    {
        Debug.Log("³ÖÐø¹¥»÷");
        Attack();
    }

    public override void exit()
    {
        Debug.Log("ÍË³ö¹¥»÷×´Ì¬");
        enemy.anim.SetBool("Is_Attacking", false);
    }

    void Attack() 
    {
        if (Vector3.Distance(enemy.transform.position, enemy.hateTarget.transform.position) < enemy.attackRange)
        {
            Debug.Log("¹¥»÷");
        }
        else 
        {
            enemy.StateChange(new MoveState(enemy));
            return;
        }
    }
}
