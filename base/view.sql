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
SELECT id_salle,SUM(nombre)/COUNT(*) as moyenne,date 
FROM nombre_salle
GROUP BY date,id_salle
;