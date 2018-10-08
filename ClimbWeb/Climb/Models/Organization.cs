using System;
using System.Collections.Generic;

namespace Climb.Models
{
    public class Organization
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }

        public List<League> Leagues { get; set; }
        public List<OrganizationUser> Members { get; set; }
    }
}