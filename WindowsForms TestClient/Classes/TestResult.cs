using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsForms_TestClient.Classes
{
    public class TestResult
    {
        public int Id { get; set; }
        public DateTime DateTest { get; set; }
        public float Mark { get; set; }
        ////////////User
        public int UserId { get; set; }
       // public virtual User Users { get; set; }
        //////////Test
        public int TestId { get; set; }
        //public virtual Test Tests { get; set; }

    }
}
