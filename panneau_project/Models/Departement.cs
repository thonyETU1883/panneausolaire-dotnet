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
    String? Id_departement;
    String? Nom_departement;

    public Departement(){}

    public Departement(String id_departement){
        this.setId_departement(id_departement);
    }


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

        return liste_luminiosite;

    }

    public double getcapacitebatterie(NpgsqlConnection liaisonbase){
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


        return capacite;
    }


    /****

        liste luminiosite avec capacite panneau et capacite batterie jusqu au coupure ou a 5h


        niveau batterie = 7 000 W
        consommation total eleve /h = 3 000 W

                dateheure      | niveau | id_departement | puissance_panneau  | niveau batterie 
            -------------------+--------+----------------+------------------+-------------------
            2023-12-01 08:00:00 |      6 | departement1   |               900 | 4900
            2023-12-01 09:00:00 |      7 | departement1   |              1050 | 2950
            2023-12-01 10:00:00 |      8 | departement1   |              1200 | 1150
            2023-12-01 11:00:00 |      9 | departement1   |              1350 | X
            2023-12-01 11:22:48 |      8 | departement1   |              1200 | 0
            2023-12-01 12:00:00 |     10 | departement1   |              1500 | X
            2023-12-01 13:00:00 |      8 | departement1   |              1200 | X
            2023-12-01 14:00:00 |      7 | departement1   |              1050 | X
            2023-12-01 15:00:00 |      6 | departement1   |               900 | X
            2023-12-01 16:00:00 |      5 | departement1   |               750 | X
            2023-12-01 17:00:00 |      4 | departement1   |               600 | X

            1h -> 3000     1150 / 3000 = 0.38  => 2023-12-01 10:00:00 + 0.38 = 2023-12-01 10:22:48
            ? <-  1150
    
    */
    public List<Luminiosite> detail_coupurewithconsommationteste(NpgsqlConnection liaisonbase,double consomation_eleve,List<Luminiosite> liste_luminiosite){
        Console.WriteLine("--------------------------------------------------");
        
        Console.WriteLine("consommation : "+consomation_eleve.ToString());
        Console.WriteLine("--------------------------------------------------");
        double capacite_batterie = this.getcapacitebatterie(liaisonbase);
        Console.WriteLine("capacite batterie : "+capacite_batterie.ToString());
        Console.WriteLine("--------------------------------------------------");

        double c_b = capacite_batterie;
        int i=0;
        double puissance_panneau = 0;
        double c_b_initial = 0;
        double heure_coupure_batterie =0;
        double heure_coupure_panneau = 0;
        List<Luminiosite> liste = new List<Luminiosite>();
        while(c_b > 0 && i<liste_luminiosite.Count){
            puissance_panneau = liste_luminiosite[i].getPuissance_panneau();
            double reste_consommationeleve = puissance_panneau-consomation_eleve;
           
            Console.WriteLine("reste : "+reste_consommationeleve.ToString());
            Console.WriteLine("c_b : "+c_b.ToString());
            c_b_initial = c_b; 
            c_b = c_b+reste_consommationeleve;
            
            liste_luminiosite[i].setReste_batterie(c_b,capacite_batterie);
            liste_luminiosite[i].setReste_panneau(reste_consommationeleve);


            c_b = liste_luminiosite[i].getReste_batterie();
            if(c_b<0 || i==(liste_luminiosite.Count-1)){
                Console.WriteLine("farany c-b :"+c_b_initial.ToString());
                Console.WriteLine("puissance panneau :"+puissance_panneau.ToString());
                Console.WriteLine("consommation eleve :"+consomation_eleve.ToString());

                heure_coupure_batterie = c_b_initial / consomation_eleve;
                heure_coupure_panneau = puissance_panneau / consomation_eleve;
                DateTime date_coupure = liste_luminiosite[i].getDateheure().AddHours(heure_coupure_batterie+heure_coupure_panneau);
                liste_luminiosite[i].setDateheure(date_coupure);
            }
            Console.WriteLine("heure : "+liste_luminiosite[i].getDateheure());
            Console.WriteLine("puissance panneau : "+liste_luminiosite[i].getReste_panneau().ToString());
            Console.WriteLine("reste batterie "+liste_luminiosite[i].getReste_batterie().ToString());
            Console.WriteLine("-----------------------------------------------------------");
            liste.Add(liste_luminiosite[i]);
            i++;
        }

        return liste;
    }   

    public DateTime coupurewithconsommationteste(NpgsqlConnection liaisonbase,double consomation_eleve,List<Luminiosite> liste_luminiosite){
        
        double capacite_batterie = this.getcapacitebatterie(liaisonbase);
        double c_b = capacite_batterie;
        int i=0;
        double puissance_panneau = 0;
        double heure_coupure_batterie = 0;
        double heure_coupure_panneau =0;
        double c_b_initial = 0;
        while(c_b > 0 && i<liste_luminiosite.Count){
            puissance_panneau = liste_luminiosite[i].getPuissance_panneau();
            double reste_consommationeleve = puissance_panneau-consomation_eleve;
            /*if(puissance_panneau<consomation_eleve && reste_consommationeleve<c_b){
                c_b = c_b - reste_consommationeleve;
            }else if(puissance_panneau<consomation_eleve && reste_consommationeleve>c_b){
                heure_coupure_batterie = c_b / consomation_eleve;
                heure_coupure_panneau = puissance_panneau / consomation_eleve;
                c_b = 0;
                DateTime date_coupure = liste_luminiosite[i].getDateheure().AddHours(heure_coupure_batterie+heure_coupure_panneau);
                liste_luminiosite[i].setDateheure(date_coupure);
                return date_coupure;
            }else if(puissance_panneau>consomation_eleve){
                double reste = puissance_panneau-consomation_eleve;
                c_b = c_b+reste;
            }*/
            Console.WriteLine("reste : "+reste_consommationeleve.ToString());
            Console.WriteLine("c_b : "+c_b.ToString());
            c_b_initial = c_b; 
            c_b = c_b+reste_consommationeleve;

            
            liste_luminiosite[i].setReste_batterie(c_b,capacite_batterie);
            liste_luminiosite[i].setReste_panneau(reste_consommationeleve);


            c_b = liste_luminiosite[i].getReste_batterie();
            if(c_b<0 || i==(liste_luminiosite.Count-1)){
                Console.WriteLine("farany c-b :"+c_b_initial.ToString());
                Console.WriteLine("puissance panneau :"+puissance_panneau.ToString());
                Console.WriteLine("consommation eleve :"+consomation_eleve.ToString());

                heure_coupure_batterie = c_b_initial / consomation_eleve;
                heure_coupure_panneau = puissance_panneau / consomation_eleve;
                DateTime date_coupure = liste_luminiosite[i].getDateheure().AddHours(heure_coupure_batterie+heure_coupure_panneau);
                liste_luminiosite[i].setDateheure(date_coupure);
            }
            Console.WriteLine("heure : "+liste_luminiosite[i].getDateheure());
            Console.WriteLine("puissance panneau : "+liste_luminiosite[i].getReste_panneau().ToString());
            Console.WriteLine("reste batterie "+liste_luminiosite[i].getReste_batterie().ToString());
            Console.WriteLine("-----------------------------------------------------------");

            i++;
        }

        puissance_panneau = liste_luminiosite[i-1].getPuissance_panneau();
        heure_coupure_batterie = c_b / consomation_eleve;
        heure_coupure_panneau = puissance_panneau / consomation_eleve;


        return liste_luminiosite[i-1].getDateheure().AddHours(heure_coupure_batterie+heure_coupure_panneau);
    }
    

    /**
         id_departement |  dateheure_coupure
        ----------------+---------------------
        departement1   | 2023-12-01 15:00:00
    */

    public DateTime getcoupuredepartementbydate(NpgsqlConnection liaisonbase,DateTime date){
       
        String sql = "SELECT dateheure_coupure FROM departement_coupure WHERE DATE(dateheure_coupure) = @datecoupure AND id_departement = @id_departement";
        if(liaisonbase == null || liaisonbase.State == ConnectionState.Closed){
            Connexion connexion = new Connexion ();
            liaisonbase = connexion.createLiaisonBase();
            liaisonbase.Open();
        }
        try{
            NpgsqlCommand cmd = new NpgsqlCommand(sql, liaisonbase);
            cmd.Parameters.AddWithValue("@datecoupure",date);
            cmd.Parameters.AddWithValue("@id_departement",this.getId_departement());
            NpgsqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read()){
                return reader.GetDateTime(0);
            }
        }catch(Exception e){
            Console.WriteLine(e.Message);
        }finally{
            if(liaisonbase != null){
                liaisonbase.Close();
            }
        }

        return DateTime.MinValue;
    }


/*

     id_departement | puissance_total | typesource
    ----------------+-----------------+------------
    departement1   |            3500 |          1
    departement1   |            1500 |          0

*/

    public double getcapacitepanneau(NpgsqlConnection liaisonbase){
        String sql = "SELECT * FROM puisssance_source_departement WHERE id_departement = @id_departement AND typesource = 0";

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

        return capacite;
    }

    
    /*
        max de consommation d'un departement
    */
    public double maxcapacite(NpgsqlConnection liaisonbase){
        double capacite_batterie = this.getcapacitebatterie(liaisonbase);
        double capacite_panneau = this.getcapacitepanneau(liaisonbase);
        return (capacite_batterie + capacite_panneau);
    }

    /*

    
    */

    public double getmoyenneelevebydate(NpgsqlConnection liaisonbase,DateTime date){
        String sql = "SELECT nombre FROM number_eleve_departement WHERE DATE(date)= @date AND id_departement = @id_departement";
        if(liaisonbase == null || liaisonbase.State == ConnectionState.Closed){
            Connexion connexion = new Connexion ();
            liaisonbase = connexion.createLiaisonBase();
            liaisonbase.Open();
        }
        double moyenne = 0;
        try{
            NpgsqlCommand cmd = new NpgsqlCommand(sql, liaisonbase);
            cmd.Parameters.AddWithValue("@date",date);
            cmd.Parameters.AddWithValue("@id_departement",this.getId_departement());

            NpgsqlDataReader reader = cmd.ExecuteReader();
            
            if(reader.Read()){
                moyenne = reader.GetDouble(0);
            }
        }catch(Exception e){
            Console.WriteLine(e.Message);
        }finally{
            if(liaisonbase != null){
                liaisonbase.Close();
            }
        }
        return moyenne;
    }

     public double getConsommationDepartement(NpgsqlConnection liaisonbase,DateTime date){
        double consommation = this.maxcapacite(liaisonbase);
        DateTime date_coupure = this.getcoupuredepartementbydate(liaisonbase,date); 
        DateTime date_coupure_teste = DateTime.MinValue;
        List<Luminiosite> liste_luminiosite = this.getLuminiosite_departement_panneau(liaisonbase,date);
        
        double a = 0;
        double b = consommation;
        double c=consommation;

        int i=200;
        TimeSpan difference = date_coupure.Subtract(date_coupure_teste);
        TimeSpan difference_positif = TimeSpan.FromTicks(Math.Abs(difference.Ticks));
        while(difference_positif.TotalMinutes > 1){
            c = (a+b)/2;
            Console.WriteLine("a : "+(a/290).ToString());
            Console.WriteLine("b : "+(b/290).ToString());
            Console.WriteLine("c : "+(c/290).ToString());
            date_coupure_teste = this.coupurewithconsommationteste(liaisonbase,c,liste_luminiosite);
            
            if(date_coupure_teste < date_coupure){
                b = c;
            }else if(date_coupure_teste > date_coupure){
                a = c;
            }

            if((b-a)<(1e-6)){
                break;
            }
            
            difference = date_coupure.Subtract(date_coupure_teste);
            difference_positif = TimeSpan.FromTicks(Math.Abs(difference.Ticks));
        }
        difference = date_coupure.Subtract(date_coupure_teste);
        difference_positif = TimeSpan.FromTicks(Math.Abs(difference.Ticks));
            
        double nombre_eleve = this.getnumberelevebynamedate(liaisonbase,date);
      
  

        return c/nombre_eleve;
    }

    public List<DateTime> getdate_exist(NpgsqlConnection liaisonbase){
        String sql = "SELECT DATE(dateheure) as date FROM luminiosite GROUP BY DATE(dateheure)";
        if(liaisonbase == null || liaisonbase.State == ConnectionState.Closed){
            Connexion connexion = new Connexion ();
            liaisonbase = connexion.createLiaisonBase();
            liaisonbase.Open();
        }

        List<DateTime> listedate = new List<DateTime>();
        try{
            NpgsqlCommand cmd = new NpgsqlCommand(sql, liaisonbase);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read()){
                DateTime d = reader.GetDateTime(0);
                listedate.Add(d);
            }
        }catch(Exception e){
            Console.WriteLine(e.Message);
        }finally{
            if(liaisonbase != null){
                liaisonbase.Close();
            }
        }
        return listedate;
    }

    public double moyenne_consommation(NpgsqlConnection liaisonbase){
        List<DateTime> listedate = this.getdate_exist(liaisonbase);
        double somme = 0;
        foreach(DateTime date in listedate){
            somme = somme + this.getConsommationDepartement(liaisonbase,date);
        }
        return somme/listedate.Count;
    }

    /*
         id_departement |    moyenne_total     | name_day
        ----------------+----------------------+-----------
        departement1   | 135.0000000000000000 | Thursday
    
    */
    public double getnumberelevebynamedate(NpgsqlConnection liaisonbase,DateTime date){
        String sql = "SELECT moyenne_total FROM nombre_salle_moyenne WHERE name_day = to_char(@date, 'Day') AND id_departement = @id_departement";

        if(liaisonbase == null || liaisonbase.State == ConnectionState.Closed){
            Connexion connexion = new Connexion ();
            liaisonbase = connexion.createLiaisonBase();
            liaisonbase.Open();
        }

        double nombre = 0;

        try{
                NpgsqlCommand cmd = new NpgsqlCommand(sql, liaisonbase);
                cmd.Parameters.AddWithValue("@date",date);
                cmd.Parameters.AddWithValue("@id_departement",this.getId_departement());

                NpgsqlDataReader reader = cmd.ExecuteReader();
                
                if(reader.Read()){
                    nombre = reader.GetDouble(0);
                }
        }catch(Exception e){
            Console.WriteLine(e.Message);
        }finally{
            if(liaisonbase != null){
                liaisonbase.Close();
            }
        }

        return nombre;
    }

    public List<Luminiosite> listecapacitepanneau(List<Luminiosite> liste,double capacitepanneau){
        List<Luminiosite> tableau = new List<Luminiosite>();
        foreach(Luminiosite l in liste){
            l.setPuissance_panneau(((l.getNiveau()*10)*capacitepanneau)/100);
            tableau.Add(l);
        }
        return tableau;
    }


    public List<Luminiosite> todoprevision(NpgsqlConnection liaisonbase,DateTime date,List<Luminiosite> liste_luminiosite){
        double nombre_eleve = this.getnumberelevebynamedate(liaisonbase,date);
        double consommation_par_eleve = this.moyenne_consommation(liaisonbase);
        double consommation_total = consommation_par_eleve * nombre_eleve;
        double capacite_panneau = this.getcapacitepanneau(liaisonbase);
        List<Luminiosite> liste = this.listecapacitepanneau(liste_luminiosite,capacite_panneau);
        return this.detail_coupurewithconsommationteste(liaisonbase,consommation_total,liste); 
    }

    public DateTime datecoupureprevision(NpgsqlConnection liaisonbase,DateTime date,List<Luminiosite> liste_luminiosite){
        double nombre_eleve = this.getnumberelevebynamedate(liaisonbase,date);
        double consommation_par_eleve = this.moyenne_consommation(liaisonbase);
        double consommation_total = consommation_par_eleve * nombre_eleve;
         double capacite_panneau = this.getcapacitepanneau(liaisonbase);
        List<Luminiosite> liste = this.listecapacitepanneau(liste_luminiosite,capacite_panneau);
        
        return this.coupurewithconsommationteste(liaisonbase,consommation_total,liste); 
    }

    public List<Departement> getalldepartement(NpgsqlConnection liaisonbase){
        String sql = "SELECT * FROM departement";
        if(liaisonbase == null || liaisonbase.State == ConnectionState.Closed){
            Connexion connexion = new Connexion ();
            liaisonbase = connexion.createLiaisonBase();
            liaisonbase.Open();
        }

        List<Departement> listedepartement = new List<Departement>();

        try{
            NpgsqlCommand cmd = new NpgsqlCommand(sql, liaisonbase);
            NpgsqlDataReader reader = cmd.ExecuteReader();
                
            while(reader.Read()){
                String id_departement = reader.GetString(0);
                String nom_departement = reader.GetString(1);
                Departement departement = new Departement(id_departement,nom_departement);
                listedepartement.Add(departement);
            }
        }catch(Exception e){
            Console.WriteLine(e.Message);
        }finally{
            if(liaisonbase != null){
                liaisonbase.Close();
            }
        }
        return listedepartement;
    }

    /*public void getdepartementbyid(NpgsqlConnection liaisonbase){
         String sql = "SELECT * FROM departement WHERE id_departement = @id_departement";
        if(liaisonbase == null || liaisonbase.State == ConnectionState.Closed){
            Connexion connexion = new Connexion ();
            liaisonbase = connexion.createLiaisonBase();
            liaisonbase.Open();
        }


        try{
            NpgsqlCommand cmd = new NpgsqlCommand(sql, liaisonbase);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            cmd.Parameters.AddWithValue("@id_departement",this.getId_departement());
                
            if(reader.Read()){
                String nom_departement = reader.GetString(1);
                this.setNom_departement(nom_departement);
            }
        }catch(Exception e){
            Console.WriteLine(e.Message);
        }finally{
            if(liaisonbase != null){
                liaisonbase.Close();
            }
        }
    }*/
}