using System;
using System.Collections.Generic;
using DailyDelights.Models;
using DailyDelights.ViewModels;

public class KMeansCluster
{
    // Orders class to represent data points
    

    // Main K-Means method
    public static List<List<Orders>> KMeans(List<Orders> orders, int k, int maxIterations)
    {
        Random random = new Random();
        List<Orders> centroids = new List<Orders>();

        // Step 1: Initialize centroids randomly from the orders
        for (int i = 0; i < k; i++)
        {
            centroids.Add(orders[random.Next(orders.Count)]);
        }

        List<List<Orders>> clusters = new List<List<Orders>>();
        for (int i = 0; i < k; i++)
        {
            clusters.Add(new List<Orders>());
        }

        // Step 2: Iteratively update clusters and centroids
        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            // Clear clusters for new assignment
            foreach (var cluster in clusters)
            {
                cluster.Clear();
            }

            // Step 2.1: Assign each order to the nearest centroid
            foreach (var order in orders)
            {
                double minDistance = double.MaxValue;
                int nearestCentroidIndex = -1;

                for (int i = 0; i < k; i++)
                {
                    double distance = order.Distance(centroids[i]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestCentroidIndex = i;
                    }
                }

                clusters[nearestCentroidIndex].Add(order);
            }

            // Step 2.2: Update centroids to the mean of their clusters
            bool centroidsChanged = false;
            for (int i = 0; i < k; i++)
            {
                var cluster = clusters[i];
                if (cluster.Count == 0) continue;

                double sumGender = 0, sumAverageOrderValue = 0, sumAverageProductValue = 0;
                foreach (var order in cluster)
                {
                    sumGender += order.Gender;
                    sumAverageOrderValue += order.AverageOrderValue;
                    sumAverageProductValue += order.AverageProductValue;
                }

                Orders newCentroid = new Orders(
                    Guid.NewGuid(), // New GUID for the centroid
                    (int)(sumGender / cluster.Count),
                    (int)(sumAverageOrderValue / cluster.Count),
                    (int)(sumAverageProductValue / cluster.Count)
                );

                if (!newCentroid.Gender.Equals(centroids[i].Gender) ||
                    !newCentroid.AverageOrderValue.Equals(centroids[i].AverageOrderValue) ||
                    !newCentroid.AverageProductValue.Equals(centroids[i].AverageProductValue))
                {
                    centroids[i] = newCentroid;
                    centroidsChanged = true;
                }
            }

            // If centroids do not change, the algorithm has converged
            if (!centroidsChanged)
            {
                break;
            }
        }

        return clusters;
    }

    public static List<Orders> GetCluster(List<Orders> orders,Guid userId)
    {
        // Create sample orders
      
        List<Orders> cluster=new List<Orders>();
        // Number of clusters (k) and maximum iterations
        int k = 2;
        int maxIterations = 100;

        // Apply K-Means Clustering
        List<List<Orders>> clusters = KMeans(orders, k, maxIterations);

        // Print the resulting clusters
        for (int i = 0; i < clusters.Count; i++)
        {
           
            foreach (var order in clusters[i])
            {
                if(order.OriginalId==userId){
                    cluster=clusters[i];
                }
            }
        }
        return cluster;
    }
}




