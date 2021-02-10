using System;
using System.Collections.Generic;

#nullable disable

namespace BookGrpcServer.Models
{
    public partial class Book
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public double Price { get; set; }
    }
}
