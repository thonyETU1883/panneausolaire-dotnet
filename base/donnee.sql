INSERT INTO salle(nom_salle) VALUES 
('S1'),
('S2')
;

INSERT INTO nombre_salle VALUES 
('salle1',120,0,'2023-12-01'),
('salle1',150,1,'2023-12-01'),
('salle2',100,0,'2023-12-01'),
('salle2',110,1,'2023-12-01')
;

INSERT INTO nombre_salle VALUES 
('salle1',130,0,'2023-12-02')
;

INSERT INTO departement(nom_departement) VALUES 
('departementA'),
('departementB')
;

INSERT INTO departement_salle VALUES
('departement1','salle1'),
('departement1','salle2')
;

INSERT INTO source VALUES 
('panneau' || nextval('panneau_sequence')::TEXT,1500,'W',0),
('panneau' || nextval('panneau_sequence')::TEXT,2000,'W',0),
('batterie' || nextval('batterie_sequence')::TEXT,4000,'W',1),
('batterie' || nextval('batterie_sequence')::TEXT,3000,'W',1)
;

INSERT INTO departement_source VALUES 
('departement1','panneau1'),
('departement1','batterie1'),
('departement1','batterie2')
;

INSERT INTO luminiosite VALUES
('2023-12-01 08:00:00',6),
('2023-12-01 09:00:00',7),
('2023-12-01 10:00:00',8),
('2023-12-01 11:00:00',9),
('2023-12-01 12:00:00',10),
('2023-12-01 13:00:00',8),
('2023-12-01 14:00:00',7),
('2023-12-01 15:00:00',6),
('2023-12-01 16:00:00',5),
('2023-12-01 17:00:00',4)
;