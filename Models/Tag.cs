﻿namespace FrontToBackMvc.Models
{
    public class Tag:BaseEntity
    {
        public string Name  { get; set; }
        public ICollection<Product> Products { get; set; } 
    }       
}
