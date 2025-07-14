using System;

namespace Trump.Models
{
    public class RequestorData
    {
        public int PNI { get; set; }
        public string Name { get; set; }
    }

    public class OurWordData_
    {
        public int Challen { get; set; }
        public DateTime RaiseDate { get; set; }
        public string Type { get; set; }
        public string PNI { get; set; }
        public string SupperType { get; set; }
        public string Status { get; set; }
    }
    public class StudentViewModel
    {
        public RequestorData Requestorv { get; set; }
        public OurWordData_ OurWordDatav { get; set; }
    }

}