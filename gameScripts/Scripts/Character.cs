using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public string characterName,career;
    public float moveSpeed, attackDamage, currentHealth, maxHealth, armorResistance, magicResistance;
    public int attackAmount;
    public int height = 0;  //该对象所在的高度层
    public Sprite illustration, sideImage;

    public GameObject normalAttack;
    //public List<SkillBase> skills;
    public SkeletonMecanim skele;
    public Animator anim;
}