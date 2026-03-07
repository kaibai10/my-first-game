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
    public Sprite illustration, sideImage;

    public GameObject normalAttack;
    public List<Skill> skills;
    public SkeletonMecanim skele;
    public Animator anim;
}