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
        return this.id_departement;
    }

    public String getNom_departement(){
        return this.nom_departement;
    }

    public List<Luminiosite> getLuminiosite_departement_panneau(NpgsqlConnection liaisonbase,DateTime date){
        String sql = "SELECT * FROM luminiosite_departement_panneau WHERE DATE(dateheure) = @dateheure AND id_departement = '@id_departement'"; 

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
            NpgsqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read()){
                
            }
        }catch(Exception e){
            Console.WriteLine(e.Message);
        }finally{
            if(liaisonbase != null){
                liaisonbase.Close();
            }
        }


        return liste_luminiosite;

    }
}