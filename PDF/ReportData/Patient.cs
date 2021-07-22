using System;

namespace DoseObjectiveCheck
{
    public class Patient
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public DateTime Birthdate { get; set; }
        public Doctor Doctor { get; set; }

    }
}