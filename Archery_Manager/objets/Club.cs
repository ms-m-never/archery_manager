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
            Archers = new List<Archer>();
        }
        public List<Archer> Archers
        {            
            get;
            set;
        }
        public bool RemoveArcher(Archer archer)
        {
            if(this.Archers==null)
            {
                return false;
            }
            else
            {
                this.Archers.Remove(archer);
                return true;
            }
        }
        public bool AddArcher(Archer archer)
        {
            if(this.Archers==null)
            {
                return false;
            }
            else
            {
                this.Archers.Add(archer);
                return true;
            }
        }
    }
}
