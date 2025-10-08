using System.ComponentModel.DataAnnotations;

namespace ev_charge_point_api.Dtos
{
    public class CancelBookingDto
    {
        // EV owner's NIC is required when the caller is not a privileged role
        public string? EvOwnerNic { get; set; }
    }
}
