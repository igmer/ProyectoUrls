using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoUrls.Models
{
    [Table("TinyUrl")]
    public class TinyUrl
    {
        [Key]
        public int Id { get; set; }
        public String Codigo { get; set; }
        public Boolean IsEnabled { get; set; }
        public String DestUrl { get; set; }
        public String Mensaje { get; set; }
    }
}
