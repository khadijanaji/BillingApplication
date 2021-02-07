using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatalogueApp.Models{
    public class Costumer{
        public int CostumerID { get; set; }
        public string Name { get; set; }
        
        public string Email { get; set; }
    }
}