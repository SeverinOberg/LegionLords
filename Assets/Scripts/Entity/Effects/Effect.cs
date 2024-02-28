using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Effect
{

    public Effect(Effects id)
    {
        ID = id;
    }

    protected bool isSingleUpdate;

    protected float duration;
    protected float timer { get; private set; }

    public Effects ID { get; protected set; }
    public int MaxStacks { get; protected set; }
    public int Stacks { get; private set; }
    
    protected float tickCooldown;
    protected float tickTimer    { get; private set; }

    protected float ticksDone    { get; private set; }

    protected Entity entity;

    public Action<Effect> OnEndEvent;

    protected virtual void OnAwake() {}
    protected virtual void OnEnd()   {}
    protected virtual void Tick()    {}

    public void Restart() => timer = 0;

    public void IncrementStack() => Stacks++;

    public IEnumerator DoExecute(Entity entity)
    {
        this.entity = entity;

        OnAwake();

        timer += Time.deltaTime;

        if (isSingleUpdate)
        {
            Update();
            yield return new WaitForSeconds(duration);
        }
        else
        {
            while (timer < duration)
            {
                Update();
                yield return null;
            }
        }

        OnEnd();
        OnEndEvent?.Invoke(this);
    }
    
    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer > tickCooldown)
        {
            tickTimer = 0;
            Tick();
            ticksDone++;
        }
    }
    
}
