using Npgsql;
using System.Data;
namespace panneau_project.Models;


/*


      dateheure      | niveau | id_departement | puissance_panneau
---------------------+--------+----------------+-------------------
 2023-12-01 08:00:00 |      6 | departement1   |               900
 2023-12-01 09:00:00 |      7 | departement1   |              1050
 2023-12-01 10:00:00 |      8 | departement1   |              1200
 2023-12-01 11:00:00 |      9 | departement1   |              1350
 2023-12-01 12:00:00 |     10 | departement1   |              1500
 2023-12-01 13:00:00 |      8 | departement1   |              1200
 2023-12-01 14:00:00 |      7 | departement1   |              1050
 2023-12-01 15:00:00 |      6 | departement1   |               900
 2023-12-01 16:00:00 |      5 | departement1   |               750
 2023-12-01 17:00:00 |      4 | departement1   |               600

**/



class Luminiosite{
    Timestamp Dateheure;
    int Niveau;
    String Id_departement;
    double Puissance_panneau;

    public Luminiosite(){}

    public Luminiosite(Timestamp dateheure,int niveau,String id_departement,double puissance_panneau){
        this.setDateheure(dateheure);
        this.setNiveau(niveau);
        this.setId_departement(id_departement);
        this.setPuissance_panneau(puissance_panneau);
    }

    public void setDateheure(Timestamp dateheure){
        this.Dateheure = dateheure;
    }

    public void setNiveau(int niveau){
        this.Niveau = niveau;
    }

    public void setId_departement(String id_departement){
        this.Id_departement = id_departement;
    }

    public void setPuissance_panneau(double puissance_panneau){
        this.Puissance_panneau = puissance_panneau;
    }

    public Timestamp getDateheure(){
        return this.Dateheure;
    }

    public int getNiveau(){
        return this.Niveau;
    }

    public String getId_departement(){
        return this.Id_departement;
    }

    public double getPuissance_panneau(){
        return this.Puissance_panneau;
    }
} 

