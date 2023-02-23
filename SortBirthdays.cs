using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CongratsApp
{
    class SortBirthdays
    {
        public string? Name { get; set; }
        public DateTime Birthdate { get; set; }
        public string? Phone { get; set; }
        public int InDays { get; set; }
        public int Year { get; set; }
        public SortBirthdays(string? N, DateTime B, string? P, int D, int Y)
        {
            Name = N;
            Birthdate = B;
            Phone = P;
            InDays = D;
            Year = Y;
        }
    }
}
