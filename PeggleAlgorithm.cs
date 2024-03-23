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
        // game handles the visuals of our algorithm running
        private Game game;

        private LevelComponent[] levels;
        private int[] population;

        public PeggleAlgorithm(Game game, int popSize, LevelComponent[] levels)
        {
            random = new Random();
            this.game = game;
            populationSize = popSize;
            this.levels = levels;
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

        public int fitness(int genome, LevelComponent level)
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

        public int[] selectionPair(List<int> population)
        {
            return new int[1];
        }

        public int mutation(int genome)
        {
            return 0;
        }

        public void main()
        {
            population = generatePopulation(populationSize);
            int[] scores = new int[populationSize];

            Thread[] threads = new Thread[populationSize];
            for (int i=0; i<populationSize; i++)
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
            foreach (int score in scores) 
            {
                System.Diagnostics.Debug.WriteLine(score);
            }
        }

    }
}