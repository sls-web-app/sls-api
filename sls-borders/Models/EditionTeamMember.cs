using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sls_borders.Models
{
    public class EditionTeamMember
    {
        public Guid Id { get; set; }

        public Guid EditionId { get; set; }
        public Edition Edition { get; set; } = default!;

        public Guid TeamId { get; set; }
        public Team Team { get; set; } = default!;

        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
