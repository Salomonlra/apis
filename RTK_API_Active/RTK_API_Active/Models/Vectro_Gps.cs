namespace RTK_API_Active.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("VECTRO_GPS")]
    public class Vectro_Gps
        {
            [Key]
            public int ID { get; set; }
            public required string EQUIPMENT { get; set; }
            public DateTime DATE { get; set; }
            public TimeSpan TIME { get; set; }
            public double? LAT { get; set; }
            public double? LON { get; set; }
            public required string SPD { get; set; }
            public required string IN_1 { get; set; }
            public required string ENC { get; set; }
            public double? VEXT { get; set; }
            public double? VINT { get; set; }
            public DateTime LOG_DATE { get; set; }
        }
    }




