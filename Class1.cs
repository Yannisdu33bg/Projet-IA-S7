using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetIA2022
{
    public class Node2 : GenericNode 
    {
        public int x;
        public int y;
        public static int objectifX;
        public static int objectifY;

        // Méthodes abstrates, donc à surcharger obligatoirement avec override dans une classe fille
        public override bool IsEqual(GenericNode N2)
        {
            Node2 N2bis = (Node2)N2;

            return (x == N2bis.x) && (y == N2bis.y);
        }

        public override double GetArcCost(GenericNode N2)
        {
            // Ici, N2 ne peut être qu'1 des 8 voisins, inutile de le vérifier
            Node2 N2bis = (Node2)N2;
            double dist = Math.Sqrt((N2bis.x-x)*(N2bis.x-x)+(N2bis.y-y)*(N2bis.y-y));
            if (Form1.matrice[x, y] == -1)
                // On triple le coût car on est dans un marécage
                dist = dist*3;
            return dist;
        }

        public override bool EndState()
        {
            return (x == Form1.xfinal) && (y == Form1.yfinal);
        }

        public override List<GenericNode> GetListSucc()
        {
            List<GenericNode> lsucc = new List<GenericNode>();

            for (int dx=-1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if ((x + dx >= 0) && (x + dx < Form1.nbcolonnes)
                            && (y + dy >= 0) && (y + dy < Form1.nblignes) && ((dx != 0) || (dy != 0)))
                        if (Form1.matrice[x + dx, y + dy] > -2)
                        {
                            Node2 newnode2 = new Node2();
                            newnode2.x = x + dx;
                            newnode2.y = y + dy;
                            lsucc.Add(newnode2);
                        }
                }

            }
            return lsucc;
        }


        public override double CalculeHCost()
        {
            //version 0

            //return 0;

            //version 1 : efficacité : 15, 123, 250
            //Principe : Ajouter du poids à la distance entre le point à analyser et le point final

            //return (Math.Sqrt(Math.Pow(x-Form1.xfinal,2)+Math.Pow(y-Form1.yfinal,2)));

            //Version 2 : efficacité : Variable en fonction du coefficient multiplicatif
            //Meilleure efficacité avec un coefficient de 12 trouvé à taton: 15, 86, 54
            //Principe: D'abord trouver les points de passage pour les définir en objectif avant de viser directement le point final

            //Récupération des coordonnées des obstacles

            if (x == Form1.xinitial - 1 && y == Form1.yinitial - 1)
            {
                objectifX = Form1.xfinal;
                objectifY = Form1.yfinal;
                //On détermine la zone à analyser
                List<int> coordX = new List<int>();
                List<int> coordY = new List<int>();
                for (int i = x; i < Form1.xfinal; i++)
                {
                    for (int j = y; j < Form1.yfinal; j++)
                    {
                        if (Form1.matrice[i, j] == -2)
                        {
                            coordX.Add(i);
                            coordY.Add(j);
                        }
                    }
                }
                //Trouver un passage
                foreach (int i in coordX)
                {
                    List<int> coordPassageX = new List<int>();
                    List<int> coordPassageY = new List<int>();
                    uint nbPassage = 0;
                    //On regarde s'il est nécessaire de passer par un passage en particulier
                    for(int j = 0; j<Form1.nblignes; j++)
                    {
                        if(Form1.matrice[i,j] != -2)
                        {
                            nbPassage++;
                            coordPassageX.Add(i);
                            coordPassageY.Add(j);
                        }
                    }
                    //Si on voit qu'il n'existe pas de passage étroit on ne fait rien 
                    if (nbPassage > 5)
                    {
                        coordPassageX = null;
                        coordPassageY = null;
                    }
                    //Sinon on va chercher par quel passage il est plus judicieux de passer
                    else
                    {
                        int indiceCheminPlusCourt = 0;
                        int indice = 0;
                        foreach(int j in coordPassageY)
                        {
                            //Pour cela on regarde la distance entre le point d'origine et les passages possibles, pour conserver au final celui le plus proche 
                            if(Math.Sqrt(Math.Pow(Form1.xinitial - i, 2) + Math.Pow(Form1.yinitial - j, 2)) < Math.Sqrt(Math.Pow(Form1.xinitial - coordPassageX[indiceCheminPlusCourt], 2) + Math.Pow(Form1.yinitial - indiceCheminPlusCourt, 2)))
                            {
                                indiceCheminPlusCourt = indice;
                            }
                            indice++;
                        }
                        //On dit que l'objectif à atteindre en premier lieu est le passage
                        objectifX = coordPassageX[indiceCheminPlusCourt];
                        objectifY = coordPassageY[indiceCheminPlusCourt];
                    }
                }
            }
            
            if (x == objectifX && y == objectifY)
            {
                objectifX = Form1.xfinal;
                objectifY = Form1.yfinal;
                Console.WriteLine("Changement : " + x);
            }

            //On calcul ensuite la distance entre l'objectif et le point en train d'être analyser
            //On multiplie ensuite par un coefficient pour ajouter du poids et de l'importance à la distance
            //On remarque que plus ce coefficient est grand, plus l'algorithme emble fonctionner
            return (Math.Sqrt(Math.Pow(x - objectifX, 2) + Math.Pow(y - objectifY, 2)))*12;

        }

        public override string ToString()
        {
            return Convert.ToString(x)+","+ Convert.ToString(y);
        }
    }
}
