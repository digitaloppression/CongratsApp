using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CongratsApp
{
    public class Birthday
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime Date { get; set; }
        public string? Phone { get; set; }

        public static implicit operator int(Birthday v)
        {
            throw new NotImplementedException();
        }
    }
}
