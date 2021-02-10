using System;
namespace BookGrpcClient.Models
{
    public class Book
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
    }
}
