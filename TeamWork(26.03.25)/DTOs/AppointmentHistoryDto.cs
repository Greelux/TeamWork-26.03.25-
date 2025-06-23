using System;

namespace HealthMeet.DTOs
{
    public class AppointmentHistoryDto
    {
        public int AppointmentId { get; set; }
        public string PatientUsername { get; set; }
        public string DoctorUsername { get; set; }
        public string SpecialtyName { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Status { get; set; }
    }
}
