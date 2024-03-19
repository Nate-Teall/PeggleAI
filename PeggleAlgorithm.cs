using System;
using System.Collections.Generic;

namespace PeggleAI
{
    // This class holds the functions required to execute the genetic algorithm for a game of Peggle.
    // The genome for this problem right now is a single float to represent the angle to shoot the ball
    // In the future, there may be additional components to one "solution", such as a time to wait.
    // The bucket may be implemented, which means that the algorithm will need to learn to time
    // the shots to land the ball in the bucket.
    public class PeggleAlgorithm : IGeneticAlgorithm<int>
    {
        Random random;

        public PeggleAlgorithm()
        {
            random = new Random();
        }

        // The random initial genomes will be any angle that the ball shooter can aim
        public int generateGenome()
        {
            // Generate a random angle within the bounds of the shooter
            return random.Next(BallShooter.getMaxLeft(), BallShooter.getMaxRight() + 1);
        }

        public List<int> generatePopulation(int size)
        {
            ArrayList<int> population = new List<int>();

            for (int i=0; i<size; i++)
            {
                population.Add(generateGenome());
            }

            return population;
        }

        public int fitness(float genome)
        {
            return 0;
        }

        public int[] selectionPair(List<int> population)
        {
            return new float[1];
        }

        public int mutation(int genome)
        {
            return 0;
        }

    }
}