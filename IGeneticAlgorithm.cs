using System;
using System.Collections.Generic;

namespace PeggleAI
{
    // A general interface that outlines what functions are required when using the genetic algorithm to solve a problem
    // The generic type for this interface is the "Genome"
    // A Genome is some genetic representation of a single possible solution to the problem
    public interface IGeneticAlgorithm<T>
    {
        // The generateGenome function will generate a single random solution to the problem
        T generateGenome();

        // The generatePopulation function will create an initial population of potential solutions.
        // This should be done by using the generateGenome function above.
        // The number of solutions within the population is given by the size parameter
        T[] generatePopulation(int size);

        // The fitness function takes in a singular genome as input.
        // Depending on the problem that is being solved, the fitness function will measure the performance of the given solution
        int fitness(T genome);

        // This function chooses the two genomes that performed the best from a given population.
        // These two genomes are the "parents" of the next generation.
        //      The genomes in the next generation will be very similar to these parents, only with slight "mutations"
        //
        // I believe that the genetic algorithm usually selects parents at random, and the fitness of each genome acts as a weight.
        // So, the better that a genome performed, the more likely they are to be selected as parents.
        // However, I think that choosing the two best performing genomes each times will give better results. 
        // I will try both ways to see which works generally better.
        T[] selectionPair(List<T> population);

        // Another important part of the genetic algorithm is the "crossover function"
        // This will combine the two parents in some way to create a new, unique solution. 
        // For the purposes of this game, I don't think that this is necessary.
        // The genomes are simply an input angle, so I don't see any way that we could "combine" two angles to create a better shot?

        // T crossoverFunction(T parentA, T parentB);

        // Given a single genome, the mutation function will slightly alter it in a random way.
        // This is how a new generation of genomes is created.
        // If any of these random changes give a more optimal result, it will become a parent
        T mutation(T genome);

    }
}