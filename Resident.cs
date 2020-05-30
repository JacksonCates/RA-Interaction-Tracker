using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAInteractionTracker
{
    [Serializable]
    class Resident
    {
        [Serializable]
        internal struct Interaction
        {
            internal DateTime date;
            internal string description;
        }
        internal string firstName;
        internal string lastName;
        private string email;
        internal List<Interaction> interactions;

        public Resident(string firstName, string lastName, string email)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            interactions = new List<Interaction>();
        }

        internal void sortInteractions()
        {
            interactions.Sort((date1, date2) => date1.date.CompareTo(date2.date));
        }
    }
}
