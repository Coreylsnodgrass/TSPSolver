using System;
using System.Collections.Generic;
using System.Linq;

namespace TSPme
{
    public class ApproxAlgo
    {
        // List to store all cities that are part of the problem
        private List<City> Cities { get; set; }
        // Dictionary to represent a Minimum Spanning Tree (MST) as adjacency list, mapping a city to its connected cities and the weight of the edge
        private Dictionary<City, List<Tuple<City, double>>> MST = new Dictionary<City, List<Tuple<City, double>>>();

        // Constructor of the ApproxAlgo class, initializes the Cities list and edges between them
        public ApproxAlgo(List<City> cities)
        {
            Cities = cities; // Set the internal Cities list to the one provided
            InitializeEdges(); // Call the method to initialize edges between all cities
        }

        // Method to ensure every city has an edge to every other city, with no duplicates
        private void InitializeEdges()
        {
            // Iterate over each city in the Cities list
            foreach (City city in Cities)
            {
                // Iterate over each city again to form edges
                foreach (City other in Cities)
                {
                    // Check if the other city is not the same and an edge does not already exist
                    if (city != other && !city.Edges.Any(e => e.Item1 == other))
                    {
                        // Add an edge from the current city to the other city
                        city.AddEdge(other);
                    }
                }
            }
        }

        // The main method to solve the TSP using an approximation algorithm
        public List<City> Solve()
        {
            // Choose the first city as the starting root city for the MST
            City root = Cities[0];
            GenerateMSTPrim(root); // Generate the MST starting from the root
            List<City> preorderPath = PreorderWalk(root); // Perform a preorder walk to visit the cities
            return CreateHamiltonianCycle(preorderPath); // Create and return the Hamiltonian cycle from the preorder path
        }

        // Method to generate the MST using Prim's algorithm
        private void GenerateMSTPrim(City root)
        {
            // Priority queue to select the next city based on the shortest edge
            var priorityQueue = new PriorityQueue<City, double>();
            // Set to keep track of cities that are already included in the MST
            var inMST = new HashSet<City>();

            // Add the root city to the priority queue with a priority of 0
            priorityQueue.Enqueue(root, 0);

            // Loop until the priority queue is empty
            while (priorityQueue.Count > 0)
            {
                // Dequeue the city with the lowest edge weight
                City current = priorityQueue.Dequeue();
                inMST.Add(current); // Mark the current city as included in the MST

                // Iterate over all edges of the current city
                foreach (var edge in current.Edges)
                {
                    City adj = edge.Item1; // The adjacent city
                    double weight = edge.Item2; // The weight of the edge to the adjacent city

                    // Check if the adjacent city is not in MST and not already in the priority queue with a lower weight
                    if (!inMST.Contains(adj) && !priorityQueue.UnorderedItems.Any(x => x.Element == adj && x.Priority <= weight))
                    {
                        // Enqueue the adjacent city with its edge weight
                        priorityQueue.Enqueue(adj, weight);
                        // Add the edge to the MST representation. If the current city is already in MST, append to its list, otherwise create a new list
                        if (MST.ContainsKey(current))
                        {
                            MST[current].Add(edge);
                        }
                        else
                        {
                            MST[current] = new List<Tuple<City, double>>() { edge };
                        }
                    }
                }
            }
        }

        // Method to perform a preorder walk of the MST
        private List<City> PreorderWalk(City root)
        {
            // List to store the order in which cities are visited
            List<City> order = new List<City>();
            // Helper method to perform the recursive preorder walk
            PreorderHelper(root, new HashSet<City>(), order);
            return order; // Return the preorder path
        }

        // Recursive helper method to perform the preorder walk
        private void PreorderHelper(City node, HashSet<City> visited, List<City> order)
        {
            visited.Add(node); // Mark the node as visited
            order.Add(node); // Add the node to the preorder path

            // Check if the node is in the MST dictionary
            if (MST.ContainsKey(node))
            {
                // Iterate over all adjacent cities in the MST
                foreach (var adj in MST[node])
                {
                    // If the adjacent city is not visited, perform a recursive walk
                    if (!visited.Contains(adj.Item1))
                    {
                        PreorderHelper(adj.Item1, visited, order);
                    }
                }
            }
        }

        // Method to create a Hamiltonian cycle from the preorder path
        private List<City> CreateHamiltonianCycle(List<City> preorderPath)
        {
            // Set to keep track of visited cities to avoid duplicates in the cycle
            HashSet<City> visited = new HashSet<City>();
            // List to store the final Hamiltonian cycle
            List<City> cycle = new List<City>();

            // Iterate over each city in the preorder path
            foreach (City city in preorderPath)
            {
                // If the city is not visited, add it to the cycle
                if (!visited.Contains(city))
                {
                    visited.Add(city); // Mark the city as visited
                    cycle.Add(city); // Add the city to the cycle
                }
            }
            cycle.Add(cycle[0]); // Add the first city at the end to close the cycle
            return cycle; // Return the Hamiltonian cycle
        }
    }
}
