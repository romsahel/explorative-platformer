using System;
using System.Collections;

namespace AssemblyCSharp
{
    public class GeneticEvolutionAlgorithm
    {
        private static System.Random random = new System.Random();
        private const int populationSize = 50;


        public static float[] getRandomIndividual(IWeapon parent, int efficiencyPercentage)
        {
            ArrayList population = getRandomPopulation(parent, (efficiencyPercentage / 100f));
            return (float[])population[random.Next(population.Count)];
        }

        public static float[] getEvolved(IWeapon parent, int efficiencyPercentage)
        {
            ArrayList population = getRandomPopulation(parent, 1f);

            float[] behaviorValues = parent.BehaviorTracker.getNormalizedBehavior();
            int maxIndex = 0;
            for (int i = 1; i < behaviorValues.Length; i++)
                if (Math.Abs(behaviorValues[i] - 50) > Math.Abs(behaviorValues[maxIndex] - 50))
                    maxIndex = i;

            int[] factorArray = getFactorArray(maxIndex);
            float[] bestIndividual = (float[])population[0];
            float bestFitness = -1;
            foreach (float[] individual in population)
            {
                float fitness = fitnessFunction(factorArray, individual);
                if (fitness > bestFitness)
                {
                    bestIndividual = individual;
                    bestFitness = fitness;
                }
            }
            return bestIndividual;
        }

        private static ArrayList getRandomPopulation(IWeapon parent, float efficiencyPercentage)
        {
            ArrayList population = new ArrayList();
            float[] parentCharacteristics = parent.getCharacteristics();
            float[] parentWeaponBias = parent.getWeaponBias();

            for (int i = 0; i < populationSize; i++)
            {
                float[] characteristics = new float[(int)IWeapon.Stats.END];
                for (int j = 0; j < (int)IWeapon.Stats.END; j++)
                {
                    characteristics[j] = parentCharacteristics[j]
                        + (float)random.NextDouble() * parentWeaponBias[j] * efficiencyPercentage;
                }
                population.Add(characteristics);
            }
            return population;
        }

        private static int[] getFactorArray(int maxIndex)
        {
            int[] result = new int[(int)IWeapon.Stats.END];
            for (int j = 0; j < (int)IWeapon.Stats.END; j++)
                result[j] = -1;
            switch (maxIndex)
            {
                // getAverageTimeToKill()
                case 0:
                    result[(int)IWeapon.Stats.SPREAD] = 1;
                    result[(int)IWeapon.Stats.NUMBEROFPROJECTILES] = 1;
                    result[(int)IWeapon.Stats.PUSHBACK] = 1;
                    break;
                // getPrecision()
                case 1:
                    result[(int)IWeapon.Stats.JAM] = 1;
                    result[(int)IWeapon.Stats.FIRERATE] = 1;
                    result[(int)IWeapon.Stats.AOE] = 1;
                    break;
                // getAverageDistance()
                case 2:
                    result[(int)IWeapon.Stats.RECOIL] = 1;
                    result[(int)IWeapon.Stats.DAMAGE] = 1;
                    result[(int)IWeapon.Stats.PIERCING] = 1;
                    break;
            }
            return result;
        }

        private static float fitnessFunction(int[] factorArray, float[] characteristics)
        {
            float fitness = 0;
            for (int i = 0; i < (int)IWeapon.Stats.END; i++)
                fitness += characteristics[i] * factorArray[i];

            return fitness;
        }
    }
}

