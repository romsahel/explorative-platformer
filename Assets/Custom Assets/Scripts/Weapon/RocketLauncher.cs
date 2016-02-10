using System;
namespace AssemblyCSharp
{
    public class RocketLauncher : IWeapon
    {
        public RocketLauncher(int efficiencyPercentage)
            : base(efficiencyPercentage)
        {
            if (isEvolving)
                evolve();
        }

        public RocketLauncher(float[] characteristics)
            : base(characteristics)
        {
        }
    }
}

