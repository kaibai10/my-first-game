using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSMBase
{
    protected Enemy enemy;
    public virtual void enter(Enemy enemy) { this.enemy = enemy; }
    public virtual EnemyFSMBase handleInput(PlayerInput input) 
    {
        if (input == PlayerInput.PRESS_J) return new AttackState(enemy);
        else if(input == PlayerInput.PRESS_W) return new MoveState(enemy);
        
        return null; 
    }
    public virtual void update() { }

    public virtual void exit() { }
}

public enum EnemyStats 
{
    Idle,   //¿ÕÏÐ×´Ì¬
    Move,   //ÒÆ¶¯×´Ì¬
    Attack, //¹¥»÷×´Ì¬
    Skill,  //¼¼ÄÜ×´Ì¬
}

public enum PlayerInput 
{
    PRESS_J,
    RELEASE_J,
    PRESS_W,
    RELEASE_W,
    DEFAULT
}
