using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookGrpcServer.Models
{
    [Table("Books")]
    public class Book
    {
        [Column("ID")]
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        [Column("Name")]
        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        [Column("Price")]
        [Required]
        public double Price { get; set; }

        [Column("Category")]
        [StringLength(100)]
        [Required]
        public string Category { get; set; }

        [Column("Author")]
        [StringLength(100)]
        [Required]
        public string Author { get; set; }
    }
}
