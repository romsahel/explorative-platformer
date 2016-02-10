using Assets.Custom_Assets.Scripts.Weapon;
using System;
namespace AssemblyCSharp
{
    public class IWeapon
    {
        public enum Stats
        {
            RECOIL = 0,
            FIRERATE,
            DAMAGE,
            AOE,
            PIERCING,
            NUMBEROFPROJECTILES,
            SPREAD,
            CLIP,
            PUSHBACK,
            JAM,
            END,
        }
        public bool isEvolving = false;
        protected float[] minimumCharacteristics;
        protected float[] characteristics;
        protected float[] weaponBias;
        protected float[] behaviorBias;

        protected int evolutionMeter;
        private bool justCreated = true;
        private int efficiencyPercentage;

        public BehaviorTracker BehaviorTracker { get; private set; }

        public IWeapon(float[] characteristics)
        {
            this.characteristics = new float[(int)Stats.END];
            this.BehaviorTracker = new BehaviorTracker();
            for (int i = 0; i < characteristics.Length; i++)
                this.characteristics[i] = characteristics[i];
        }

        public IWeapon(int efficiencyPercentage)
        {
            this.efficiencyPercentage = efficiencyPercentage;
            this.BehaviorTracker = new BehaviorTracker();
            this.characteristics = new float[(int)Stats.END];
            
            this.weaponBias = new float[(int)Stats.END];
            for (int j = 0; j < (int)IWeapon.Stats.END; j++)
                this.weaponBias[j] = 5f;

            this.minimumCharacteristics = new float[(int)Stats.END];
            this.minimumCharacteristics[(int)Stats.NUMBEROFPROJECTILES] = 1;
        }


        public float[] getCharacteristics()
        {
            return characteristics;
        }

        public float getCharacteristic(IWeapon.Stats stat)
        {
            return characteristics[(int)stat];
        }
        public float[] getWeaponBias()
        {
            return weaponBias;
        }

        public float[] getBehaviorBias()
        {
            return behaviorBias;
        }

        public int changeEvolution(bool progress)
        {
            if (progress)
            {
                if (++evolutionMeter > 15)
                    this.evolve();
            }
            else if (evolutionMeter > 0)
                evolutionMeter--;
            return evolutionMeter;
        }

        public void evolve()
        {
            BehaviorTracker.print();
            if (justCreated)
            {
                this.characteristics = GeneticEvolutionAlgorithm.getRandomIndividual(this, this.efficiencyPercentage);
                for (int i = 0; i < characteristics.Length; i++)
                    this.characteristics[i] = Math.Max(this.characteristics[i], this.minimumCharacteristics[i]);
            }
            else
                this.characteristics = GeneticEvolutionAlgorithm.getEvolved(this, this.efficiencyPercentage);
            justCreated = false;
            evolutionMeter = 0;
        }

        public String getString()
        {
            String str = "";
            for (int i = 0; i < characteristics.Length; i++)
            {
                str += ((Stats)i).ToString() + ": " + characteristics[i] + " ; ";
            }
            return str;
        }

        protected void setWeaponBias(Stats stat, float value)
        {
            this.weaponBias[(int)stat] = value;
        }
    }
}

