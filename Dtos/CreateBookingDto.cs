using System;
using System.ComponentModel.DataAnnotations;

namespace ev_charge_point_api.Dtos
{
    public class CreateBookingDto
    {
        [Required]
        public string EvOwnerNic { get; set; } = string.Empty;

        [Required]
        public string StationId { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        public int SlotId { get; set; }

        [Required]
        public DateTime ReservationDateTime { get; set; }
    }
}
