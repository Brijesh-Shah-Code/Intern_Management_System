using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InternApp.Models
{
    public class t_Topics
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Topic id is required")]
        [Display(Name = "Topic Id")]
        public int? c_TopicId { get; set; }

        [Required(ErrorMessage = "Topic name is required")]
        [StringLength(100, ErrorMessage = "Topic name cannot be longer than 100 characters")]
        [Display(Name = "Topic Name")]
        public string? c_TopicName { get; set; }
    }
}