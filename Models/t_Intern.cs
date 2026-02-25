using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InternApp.Models
{
    public class t_Intern
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Intern Id")]
        public int c_InternId { get; set; }

        [StringLength(100, ErrorMessage = "Intern name cannot be longer than 100 characters")]
        [Required(ErrorMessage = "Please enter intern name")]
        [Display(Name = "Intern Name")]
        public string? c_InternName { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Please select gender")]
        // [RegularExpression("^(M|F)$", ErrorMessage = "Gender should be either 'M' for Male or 'F' for Female")]
        public string? c_Gender { get; set; }

        [Required(ErrorMessage = "Please select topic")]
        public int c_TopicId { get; set; }

        [Required(ErrorMessage = "Please select date")]
        [DataType(DataType.Date)]
        [Display(Name = "Presentation Date")]
        public DateTime? c_PresentationDate { get; set; }

        [Display(Name = "Presentation Status")]
        public bool c_IsPresented { get; set; }

        [StringLength(3000)]
        [Display(Name = "Upload Topic Image")]
        public string? ImagePath { get; set; }

        public IFormFile? ImageFile { get; set; }

        // public t_Topics? AssignedTopic { get; set; }

        public string? c_TopicName { get; set; }

    }
}