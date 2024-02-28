using System.Collections.Generic;

public class EffectController
{
    public List<Effect> Effects = new();

    public void Add(Entity entity, Effect effect)
    {
        foreach (Effect e in Effects)
        {
            if (e.ID == effect.ID)
            {
                e.Restart();
                if (e.MaxStacks < e.Stacks)
                {
                    e.IncrementStack();
                }
                return;
            }
        }

        entity.StartCoroutine(effect.DoExecute(entity));

        effect.OnEndEvent += Remove;
        Effects.Add(effect);
    }

    public void Remove(Effect effect)
    {
        if (Effects.Find((e) => e == effect) != null)
        {
            Effects.Remove(effect);
            effect.OnEndEvent -= Remove;
        }
    }
    
}
