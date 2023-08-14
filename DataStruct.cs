using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newHSBGcheck
{
    internal class BGstruct
    {
        public string accountid { get; set; }
        public int rank { get; set; }
        public int rating { get; set; }
    }
    internal class Users
    {
        public string Name { get; set; }
        public int Rank { get; set; }
        public List<Rating> ratings { get; set; }
    }
    public class Rating
    {
        public int rating { get; set; }
        public DateTime date { get; set; }
    }
}
