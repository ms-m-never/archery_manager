using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archery_Manager.objets
{
    public class Score
    {
        public Score(DateTime date, string type, string distance, string commentaire = null)
        {
            this.date = date;
            this.type = type;
            this.distance = distance;
            this.total = 0;
        }
        public Score() { }
        public int total
        {
            get;
            set;
        }
        public string type
        {
            get;
            set;
        }
        public DateTime date // A changer a datetime plus tard
        {
            get;
            set;
        }
        public string distance
        {
            get;
            set;
        }
        public string commentaire
        {
            get;
            set;
        }
        public List<Volee> TTvolee
        {
            get;
            set;
        }
    }

}
