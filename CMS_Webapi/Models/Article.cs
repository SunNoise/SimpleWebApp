using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CMS_Webapi.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public bool Reviewed { get; set; }

        [Required]
        public bool Approved { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int ReviewerId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [ForeignKey("AuthorId")]
        public virtual User Author { get; set; }

        [ForeignKey("ReviewerId")]
        public virtual User Reviewer { get; set; }
    }
}