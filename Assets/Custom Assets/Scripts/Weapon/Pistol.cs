using System;
namespace AssemblyCSharp
{
    public class Pistol : IWeapon
    {
        public Pistol(int efficiencyPercentage)
            : base(efficiencyPercentage)
        {
            minimumCharacteristics[(int)Stats.FIRERATE] = 1.5f;
            minimumCharacteristics[(int)Stats.RECOIL] = 1f;
            minimumCharacteristics[(int)Stats.DAMAGE] = 4f;

            weaponBias[(int)Stats.NUMBEROFPROJECTILES] = 0.5f;
            weaponBias[(int)Stats.SPREAD] = 8f;
            weaponBias[(int)Stats.PUSHBACK] = 0.5f;
            weaponBias[(int)Stats.DAMAGE] = 5f;
            weaponBias[(int)Stats.JAM] = 0.1f;

            if (isEvolving)
                base.evolve();
        }
        public Pistol(float[] characteristics)
            : base(characteristics)
        {
        }
    }
}

