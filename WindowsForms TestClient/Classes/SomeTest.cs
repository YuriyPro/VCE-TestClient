using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsForms_TestClient.Classes
{
    public class SomeTest
    {
        public string TestName { get; set; }
        public string Subject { get; set; }
        public string Author { get; set; }
        public List<Question> quest = new List<Question>();
    }
}
