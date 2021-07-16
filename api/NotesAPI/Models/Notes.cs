using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NotesAPI.Models {
    public class Notes {

        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        [Required]
        public string Note { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public DateTime LastModified { get; set; }

    }
}
