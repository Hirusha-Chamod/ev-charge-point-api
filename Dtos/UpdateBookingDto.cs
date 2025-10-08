using System;
using System.ComponentModel.DataAnnotations;
using ev_charge_point_api.Models;

namespace ev_charge_point_api.Dtos
{
    public class UpdateBookingDto
    {
        public BookingStatus? Status { get; set; }

        public DateTime? ReservationDateTime { get; set; }
    }
}
