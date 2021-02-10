using System.ComponentModel;

namespace ERSB.Models
{
    public class Student
    {
        [DisplayName("Name")]
        public string Name{get;set;}

        [DisplayName("Father's Name")]
        public string FatherName{get;set;}
        [DisplayName("Mother's Name")]
        public string MotherName{get;set;}
        [DisplayName("Roll No.")]
        public string RollNo{get;set;}
        [DisplayName("Enrollment No.")]
        public string EnrollmentNo { get;set;}
        [DisplayName("Result")]
        public string Result{get;set;}
        [DisplayName("Result Date")]
        public string ResultDate{get;set;}
        [DisplayName("SGPA")]
        public string Sgpa{get;set;}
        [DisplayName("CGPA")]
        public string Cgpa{get;set;}

    }
}
