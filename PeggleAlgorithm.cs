using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

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

        private int populationSize;
        // game handles the visuals of our algorithm running
        private Game game;

        public PeggleAlgorithm(Game game, int popSize)
        {
            random = new Random();
            this.game = game;
            populationSize = popSize;
        }

        // The random initial genomes will be any angle that the ball shooter can aim
        public int generateGenome()
        {
            // Generate a random angle within the bounds of the shooter
            return random.Next(BallShooter.getMaxLeft(), BallShooter.getMaxRight() + 1);
        }

        public int[] generatePopulation(int size)
        {
            int[] population = new int[size];

            for (int i=0; i<size; i++)
            {
                population[i] = generateGenome();
            }

            return population;
        }

        public int fitness(int genome)
        {
            // This function will launch the ball at a given angle, and return the result of the shot.
            return 0;
        }

        public int[] selectionPair(List<int> population)
        {
            return new int[1];
        }

        public int mutation(int genome)
        {
            return 0;
        }

    }
}