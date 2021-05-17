using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Deposits")]
    public class Deposit
    {
        public int Id { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public int Amount { get; set; } = 0; 
        public DateTime DateOfDeposit { get; set; }  = DateTime.Now;

        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }
    }
}