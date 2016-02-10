using System;
namespace AssemblyCSharp
{
    public class Shotgun : IWeapon
    {
        public Shotgun(int efficiencyPercentage)
            : base(efficiencyPercentage)
        {

            minimumCharacteristics[(int)Stats.FIRERATE] = 0.5f;
            minimumCharacteristics[(int)Stats.NUMBEROFPROJECTILES] = 3f;
            minimumCharacteristics[(int)Stats.SPREAD] = 0.5f;
            minimumCharacteristics[(int)Stats.RECOIL] = 5f;
            minimumCharacteristics[(int)Stats.DAMAGE] = 10f;

            weaponBias[(int)Stats.NUMBEROFPROJECTILES] = 3f;
            weaponBias[(int)Stats.SPREAD] = 15f;
            weaponBias[(int)Stats.DAMAGE] = 10f;
            weaponBias[(int)Stats.RECOIL] = 10f;

            if (isEvolving)
                evolve();
        }

        public Shotgun(float[] characteristics)
            : base(characteristics)
        {
        }
    }
}

