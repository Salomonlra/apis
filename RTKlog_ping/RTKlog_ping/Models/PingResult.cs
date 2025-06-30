namespace RTKlog_ping.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("log_PingResult")]
    public class PingResult
    {
        [Key]
        public int ID_Consecutive { get; set; }

        [Required]
        public required string Equipment { get; set; }

        public int Ping_Receive_Rate { get; set; }

        public DateTime Ping_DateTime { get; set; }
    }

}
