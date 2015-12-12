using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archery_Manager.objets
{
   public class Club 
    {
        public Club()
        {
            Donnees = new List<Archer>();
        }
        public List<Archer> Donnees
        {            
            get;
            set;
        }
    }
}
