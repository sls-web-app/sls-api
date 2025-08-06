using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sls_borders.DTO.EditionDto
{
    public class CreateEditionDto
    {
        public int Number { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Organizer { get; set; } = null!;
    }
}
