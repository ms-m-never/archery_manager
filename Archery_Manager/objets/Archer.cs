using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archery_Manager.objets
{
    public class Archer
    {
        public Archer(string nom, string categorie, string arme, string photo)
        {
            this.Nom = nom;
            this.Categorie = categorie;
            this.Arme = arme;
            this.Photo = photo;

        }
        public Archer()
        {

        }
        public string Nom
        {
            get;
            set;
        }
        public string Categorie
        {
            get;
            set;
        }
       public string Photo
        {
            get;
            set;
        }
        public string Arme
        {
            get;
            set;
        }
        public List<Score> Perf
        {
            get;
            set;
        }
        public void Add(Score point) {
            if (this.Perf == null)
            {

                this.Perf = new List<Score> { point };
            }
            else {
                this.Perf.Add(point);
            }
        }
        public void RemovePoint(Score point)
        {

            this.Perf.Remove(point);
            
        }
    }
}
