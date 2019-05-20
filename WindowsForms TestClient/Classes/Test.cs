using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsForms_TestClient.Classes
{
    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PathToFile { get; set; }
        //////////////Subject
        public int SubjectId { get; set; }
        //public virtual Subject Subjects { get; set; }

        //public ICollection<TestResult> TestResults { get; set; }

        //public Test()
        //{
        //    TestResults = new List<TestResult>();
        //}

        //public override string ToString()
        //{
        //    return Name;
        //}
    }
}
