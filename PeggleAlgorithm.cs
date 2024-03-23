using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using System.Threading;

namespace PeggleAI
{
    // This class holds the functions required to execute the genetic algorithm for a game of Peggle.
    // The genome for this problem right now is a single float to represent the angle to shoot the ball
    // In the future, there may be additional components to one "solution", such as a time to wait.
    // The bucket may be implemented, which means that the algorithm will need to learn to time
    // the shots to land the ball in the bucket.
    public class PeggleAlgorithm //: IGeneticAlgorithm<int>
    {
        Random random;

        private int populationSize;
        private int generationCount;
        // game handles the visuals of our algorithm running
        private Game game;

        private LevelComponent[] levels;
        private int[] population;
        // Determines the range of values that a mutated shot could be
        // For example, a mutation factor of 10 means that the shot could be changed by +/- 5 degrees 
        private const int mutationFactor = 10;

        public PeggleAlgorithm(Game game, int popSize, int genCount, LevelComponent[] levels)
        {
            random = new Random();
            this.game = game;
            populationSize = popSize;
            generationCount = genCount;
            this.levels = levels;
        }

        // The random initial genomes will be any angle that the ball shooter can aim
        public double generateGenome()
        {
            // Generate a random angle within the bounds of the shooter
            return random.Next(BallShooter.getMaxLeft(), BallShooter.getMaxRight() + 1);
        }

        public double[] generatePopulation(int size)
        {
            double[] population = new double[size];

            for (int i=0; i<size; i++)
            {
                population[i] = generateGenome();
            }

            return population;
        }

        // This function generates all but the first population. It creates a new set of genomes by mutating the parents
        private double[] generateFromParents(int size, double[] parents)
        {
            double[] population = new double[size];
            population[0] = parents[0];
            population[1] = parents[1];

            int parentPoolSize = (populationSize - 2) / 2; // Please use even numbers for population sizes so each remaining size can be equal... 
            for (int i = 2; i < 2 + parentPoolSize; i++)
            {
                // Fill population with children of parent 1
                population[i] = mutation(parents[0]);
            }

            for (int i = 2 + parentPoolSize; i < populationSize; i++)
            {
                // Fill population with children of parent 2
                population[i] = mutation(parents[1]);
            }

            return population;
        }

        public int fitness(double genome, LevelComponent level)
        {
            // This function will launch the ball at a given angle.
            // When the simulation finishes, the levelComponent will tell the algorithm what the score was.

            System.Diagnostics.Debug.WriteLine("Shot stared!");
            EventWaitHandle handle = new EventWaitHandle(false, EventResetMode.ManualReset);
            level.finishHandle = handle;
            level.shootAtAngle(genome);

            // Wait until the game finished simulating
            handle.WaitOne();
            System.Diagnostics.Debug.WriteLine("Shot finished!");

            return level.previousShotScore;

        }

        // Performs the fitness function on an entire population
        private int[] gradePopulation(double[] population)
        {
            int[] scores = new int[populationSize];

            Thread[] threads = new Thread[populationSize];
            for (int i = 0; i < populationSize; i++)
            {
                Thread t = new Thread(
                    (i) =>
                    {
                        System.Diagnostics.Debug.WriteLine(i);
                        scores[(int)i] = fitness(population[(int)i], levels[(int)i]);
                    }
                );

                threads[i] = t;
                t.Start(i);
            }

            foreach (Thread t in threads)
            {
                t.Join();
            }

            System.Diagnostics.Debug.WriteLine("All shots finished");
            return scores;
        }

        public double[] selectionPair(double[] population, int[] scores)
        {
            int high_score = 0;
            int second_high_score = 0;

            int highest_i = 0;
            int second_highest_i = 0;
            double[] pair = new double[2];

            for (int i=0; i<populationSize; i++)
            {
                if (scores[i] > high_score)
                {
                    second_high_score = high_score;
                    high_score = scores[i];

                    second_highest_i = highest_i;
                    highest_i = i;
                } 
                else if (scores[i] > second_high_score)
                {
                    second_high_score = scores[i];
                    second_highest_i = i;
                }
            }

            pair[0] = population[highest_i];
            pair[1] = population[second_highest_i];

            System.Diagnostics.Debug.WriteLine(scores[highest_i] + " " + scores[second_highest_i]);

            return pair;
        }

        public double mutation(double genome)
        {
            // As a first experiment, mutations will modify the shot angle by anywhere between +/- 5 degrees
            return genome + ( (random.NextDouble() * mutationFactor) - (mutationFactor / 2) );
        }

        public void main()
        {
            double[] population = generatePopulation(populationSize);
            int[] scores;
            for (int i=0; i<generationCount; i++)
            {
                scores = gradePopulation(population);

                double[] parents = selectionPair(population, scores);
                System.Diagnostics.Debug.WriteLine(parents[0] + " " + parents[1]);

                printPopulation(population);

                // After grading this population and selecting the parents, create a new population from them.
                population = generateFromParents(populationSize, parents);
            }
        }

        private void printPopulation(double[] population)
        {
            foreach (int genome in population)
            {
                System.Diagnostics.Debug.Write(genome + " ");
            }
            System.Diagnostics.Debug.WriteLine("");
        }

    }
}