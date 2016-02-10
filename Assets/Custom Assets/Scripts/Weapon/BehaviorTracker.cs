using System;
using UnityEngine;

namespace Assets.Custom_Assets.Scripts.Weapon
{
    public class BehaviorTracker
    {
        private int numEnemiesHit;
        private int numEnemiesKilled;
        private int numShotsFired;

        private float sumDistanceFromEnemies;
        private float sumTimeToKill;

        public void signalShooting()
        {
            numShotsFired++;
        }

        public void signalEnemyHit(Vector2 pos1, Vector2 pos2, float time)
        {
            numEnemiesHit++;
            var v = (Vector2.Distance(pos1, pos2)) * 30;
            sumDistanceFromEnemies += v;

            if (time > 0f)
            {
                sumTimeToKill += time;
                numEnemiesKilled++;
                GUIDisplayer.killCounter++;
            }
        }

        public void print()
        {
            Debug.Log("Distance: " + getNormalizedBehavior()[2] + " - Precision: " + getNormalizedBehavior()[1] + " - Time to kill: " + getNormalizedBehavior()[0]);
        }

        public float getAverageTimeToKill()
        {
            if (numEnemiesKilled == 0)
                return 0;
            return (sumTimeToKill / numEnemiesKilled);
        }

        public int getPrecision()
        {
            if (numShotsFired == 0)
                return 100;
            return (numEnemiesHit * 100 / numShotsFired);
        }

        public float getAverageDistance()
        {
            if (numEnemiesHit == 0)
                return 0;
            return (sumDistanceFromEnemies / numEnemiesHit);
        }

        public float[] getNormalizedBehavior()
        {
            return new float[] { getAverageTimeToKill() * 10, getPrecision(), getAverageDistance() };
        }
    }
}
