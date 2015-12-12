using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archery_Manager.objets
{
    public class Archer
    {
        public Archer(string name, string cat, string arme, string photo)
        {
            this.name = name;
            this.cat = cat;
            this.arme = arme;
            this.photo = photo;

        }
        public Archer()
        {

        }
        public string name
        {
            get;
            set;
        }
        public string cat
        {
            get;
            set;
        }
        //rajouter public pour pouvoir serializer, trouver comment enregistrer
        /* Windows.UI.Xaml.Media.ImageSource photo
         {
             get;
             set;
         }*/

       public string photo
        {
            get;
            set;
        }
        public string arme
        {
            get;
            set;
        }
        public List<Score> perf
        {
            get;
            set;
        }

        public static void Add(Archer tireur, Score point) {
            if (tireur.perf == null)
            {

                tireur.perf = new List<Score> { point };
            }
            else {
                tireur.perf.Add(point);
            }


        }

        public static void Remove(Archer tireur, Score point)
        {

            tireur.perf.Remove(point);
            
        }
    }
}
