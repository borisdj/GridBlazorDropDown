using System;
using System.ComponentModel.DataAnnotations;

namespace GridBlazorDropDown.Data
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Remark { get; set; }

        public bool IsNotActive { get; set; }
    }
}
