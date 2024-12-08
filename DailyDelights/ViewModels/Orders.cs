using System;

namespace DailyDelights.ViewModels;

public class Orders
    {
        public Guid OriginalId { get; private set; } // Original GUID
        public int EncodedId { get; private set; }   // Encoded integer ID
        public int Gender { get; set; }
        public double AverageOrderValue { get; set; }
        public double AverageProductValue { get; set; }

        public Orders(Guid guid, int gender, double averageOrderValue, double averageProductValue)
        {
            OriginalId = guid; // Store original GUID
            EncodedId = guid.GetHashCode(); // Encode GUID into an int
            Gender = gender;
            AverageOrderValue = averageOrderValue;
            AverageProductValue = averageProductValue;
        }

        // Method to calculate the Euclidean distance between two Orders
        public double Distance(Orders other)
        {
            return Math.Sqrt(
                Math.Pow(this.Gender - other.Gender, 2) +
                Math.Pow(this.AverageOrderValue - other.AverageOrderValue, 2) +
                Math.Pow(this.AverageProductValue - other.AverageProductValue, 2)
            );
        }

        public override string ToString()
        {
            return $"GUID: {OriginalId}, EncodedId: {EncodedId}, Gender: {Gender}, " +
                   $"AverageOrderValue: {AverageOrderValue}, AverageProductValue: {AverageProductValue}";
        }
    }