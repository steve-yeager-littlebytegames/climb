using System;
using Climb.Data;

namespace Climb.Models
{
    public class OrganizationUser
    {
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public string UserID { get; set; }
        public bool HasLeft { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsOwner { get; set; }

        public Organization Organization { get; set; }
        public ApplicationUser User { get; set; }
    }
}