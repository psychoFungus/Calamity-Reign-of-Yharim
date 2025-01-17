using UnityEngine;
public class DummyAI : NPC
{
    public override void SetDefaults()
    {
        damage = 10;
        NPCName = "Dummy";
        lifeMax = 100;
        life = lifeMax;
        healthBar.SetMaxHealth(lifeMax);
    }
    public override void AI()
    {
        if (target != null)
        {
            velocity.x = DirectionTo(target.transform.position).x * 0.05f;

            if (1 == GetTargetDirectionX()) //for looking at player
                gameObject.transform.localScale = new Vector3(1, 1, 1);
            else
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
