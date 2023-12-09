using Npgsql;
using System.Data;
namespace panneau_project.Models;

/*

 id_departement | nom_departement
----------------+-----------------
 departement1   | departementA
 departement2   | departementB


*/


class Departement {
    String Id_departement;
    String Nom_departement;

    public Departement(){}

    public Departement(String id_departement,String nom_departement){
        this.setId_departement(id_departement);
        this.setNom_departement(nom_departement);   
    }

    public void setId_departement(String id_departement){
        this.Id_departement = id_departement;
    }

    public void setNom_departement(String nom_departement){
        this.Nom_departement = nom_departement;
    }

    public String getId_departement(){
        return this.Id_departement;
    }

    public String getNom_departement(){
        return this.Nom_departement;
    }


    /*
        liste luminiosite avec les puissances des panneaux solaires d un departement

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

    */

    public List<Luminiosite> getLuminiosite_departement_panneau(NpgsqlConnection liaisonbase,DateTime date){
        String sql = "SELECT * FROM luminiosite_departement_panneau WHERE DATE(dateheure) = @dateheure AND id_departement = @id_departement"; 

        if(liaisonbase == null || liaisonbase.State == ConnectionState.Closed){
            Connexion connexion = new Connexion ();
            liaisonbase = connexion.createLiaisonBase();
            liaisonbase.Open();
        }


        List<Luminiosite> liste_luminiosite = new List<Luminiosite>();

        try{
            NpgsqlCommand cmd = new NpgsqlCommand(sql, liaisonbase);
            cmd.Parameters.AddWithValue("@dateheure",date);
            cmd.Parameters.AddWithValue("@id_departement",this.getId_departement());
            
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("classe : Departement");
            Console.WriteLine("nom fonction : getLuminiosite_departement_panneau");
            Console.WriteLine("requete : SELECT * FROM luminiosite_departement_panneau WHERE DATE(dateheure) = '"+date+"' AND id_departement ='"+this.getId_departement()+"'");

            NpgsqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read()){
                Luminiosite luminiosite = new Luminiosite();
                luminiosite.setDateheure(reader.GetDateTime(reader.GetOrdinal("dateheure")));
                luminiosite.setNiveau(reader.GetInt32(1));
                luminiosite.setId_departement(reader.GetString(2));
                luminiosite.setPuissance_panneau(reader.GetDouble(3));
                liste_luminiosite.Add(luminiosite);
            }
        }catch(Exception e){
            Console.WriteLine(e.Message);
        }finally{
            if(liaisonbase != null){
                liaisonbase.Close();
            }
        }

        Console.WriteLine("reponse : ");

        foreach (Luminiosite element in liste_luminiosite)
        {
            Console.WriteLine(element.getDateheure()+"  "+ element.getNiveau().ToString()+"  "+element.getId_departement()+"  "+element.getPuissance_panneau().ToString());
        }

        return liste_luminiosite;

    }

    public double getcapacitepanneau(NpgsqlConnection liaisonbase){
        String sql = "SELECT * FROM puisssance_source_departement WHERE id_departement = @id_departement AND typesource = 1";

        if(liaisonbase == null || liaisonbase.State == ConnectionState.Closed){
            Connexion connexion = new Connexion ();
            liaisonbase = connexion.createLiaisonBase();
            liaisonbase.Open();
        }

        double capacite = 0;

        try{
            NpgsqlCommand cmd = new NpgsqlCommand(sql, liaisonbase);
            cmd.Parameters.AddWithValue("@id_departement",this.getId_departement());

            NpgsqlDataReader reader = cmd.ExecuteReader();
            
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("classe : Departement");
            Console.WriteLine("nom fonction : getcapacitepanneau");
            Console.WriteLine("requete : SELECT * FROM puisssance_source_departement WHERE id_departement = '"+this.getId_departement()+"' AND typesource = 1");

            if(reader.Read()){
                capacite = reader.GetDouble(1);
            }

        }catch(Exception e){
            Console.WriteLine(e.Message);
        }finally{
            if(liaisonbase != null){
                liaisonbase.Close();
            }
        }

        Console.WriteLine("reponse : "+capacite.ToString());

        return capacite;
    }
}