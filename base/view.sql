--View pour calculer la moyenne d eleve par jour

/*
    nombre_salle :
         id_salle | nombre | journee |    date
        ----------+--------+---------+------------
        salle1   |    120 |       0 | 2023-12-01
        salle1   |    150 |       1 | 2023-12-01
        salle2   |    100 |       0 | 2023-12-01
        salle2   |    110 |       1 | 2023-12-01


    resultat view  : 
         id_salle | moyenne |    date
        ----------+--------+---------+
        salle1    | 135     | 2023-12-01
        salle2    | 105     | 2023-12-01
*/


--moyenne  =  somme(nombre) / nombre de ligne


CREATE VIEW nombre_salle_view AS 
SELECT id_salle,SUM(nombre)/COUNT(*) as moyenneday,date 
FROM nombre_salle
GROUP BY date,id_salle
;


/*
    departement  : 
         id_departement | nom_departement
        ----------------+-----------------
        departement1   | departementA

    departement_salle : 
         id_departement | id_salle
        ----------------+----------
        departement1   | salle1
        departement1   | salle1

    resultat 
         id_departement | moyenne_total |  name_day 
        ----------------+---------------+--------------
        departement1        120             Friday
*/

CREATE VIEW nombre_salle_moyenne AS 
SELECT departement_salle.id_departement,SUM(moyenneday)/COUNT(*) as moyenne_total,to_char(date, 'Day') as name_day 
FROM nombre_salle_view 
JOIN departement_salle ON departement_salle.id_salle = nombre_salle_view.id_salle
GROUP BY name_day,departement_salle.id_departement
;



/*
    departement  : 
         id_departement | nom_departement
        ----------------+-----------------
        departement1   | departementA

    source  :
         id_source | capacite | unite | typesource
        -----------+----------+-------+------------
        panneau1  |     1500 | W     |          0
        panneau2  |     2000 | W     |          0
        batterie1 |     4000 | W     |          1
        batterie2 |     3000 | W     |          1

    departement_source : 
         id_departement | id_source
        ----------------+-----------
        departement1   | panneau1
        departement1   | batterie1
        departement1   | batterie2

    resultat : 
         id_departement | puissance_total | typesource
        ----------------+-----------------+------------
        departement1   |            3500 |          1
        departement1   |            1500 |          0
*/

CREATE VIEW puisssance_source_departement AS
SELECT id_departement,(SUM(capacite*(pourcentage/100))) as puissance_total,typesource 
FROM departement_source
JOIN source ON departement_source.id_source = source.id_source
GROUP BY id_departement,typesource
;

/*
    luminosite :
              dateheure      | niveau
        ---------------------+--------
        2023-12-01 08:00:00 |      6
        2023-12-01 09:00:00 |      7
        2023-12-01 10:00:00 |      8
        2023-12-01 11:00:00 |      9
        2023-12-01 12:00:00 |     10
        2023-12-01 13:00:00 |      8
        2023-12-01 14:00:00 |      7
        2023-12-01 15:00:00 |      6
        2023-12-01 16:00:00 |      5
        2023-12-01 17:00:00 |      4

    puisssance_source_departement : 
        id_departement | puissance_total | typesource
        ---------------+----------------+------------
        departement1   |            7000 |          1
        departement1   |            1500 |          0


    resultat : 

    dateheure       | niveau | id_departement | puissance_panneau |
--------------------+--------+----------------+-----------------+
2023-12-01 08:00:00 |      6 | departement1   | 900
2023-12-01 09:00:00 |      7 | departement1   | 1050
2023-12-01 10:00:00 |      8 | departement1   | 1200
2023-12-01 11:00:00 |      9 | departement1   | ...
2023-12-01 12:00:00 |     10 | departement1   | ...
2023-12-01 13:00:00 |      8 | departement1   | ...
2023-12-01 14:00:00 |      7 | departement1   |
2023-12-01 15:00:00 |      6 | departement1   |
2023-12-01 16:00:00 |      5 | departement1   |
2023-12-01 17:00:00 |      4 | departement1   |
*/


CREATE VIEW Luminiosite_departement_panneau AS
SELECT
  luminiosite.dateheure,luminiosite.niveau,
  puisssance_source_departement.id_departement,
  (puisssance_source_departement.puissance_total*luminiosite.niveau*10)/100 as puissance_panneau
FROM
  luminiosite CROSS JOIN puisssance_source_departement
WHERE puisssance_source_departement.typesource = 0  
;