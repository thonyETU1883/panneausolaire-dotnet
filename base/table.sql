--base de donnee :  postgres
--nom base de donne : panneausolaire

CREATE TABLE salle (
    id_salle VARCHAR(100) DEFAULT 'salle' || nextval('salle_sequence')::TEXT PRIMARY KEY,
    nom_salle VARCHAR(50)
);

CREATE TABLE departement (
    id_departement VARCHAR(100) DEFAULT 'departement' || nextval('departement_sequence')::TEXT PRIMARY KEY,
    nom_departement VARCHAR(50)
);

CREATE TABLE departement_salle (
    id_departement VARCHAR(100),
    id_salle VARCHAR(100),
    FOREIGN KEY(id_departement) REFERENCES departement(id_departement),
    FOREIGN KEY(id_salle) REFERENCES salle(id_salle)
);

CREATE TABLE nombre_salle (
    id_salle VARCHAR(100),
    nombre INTEGER,
    journee INTEGER,
    date DATE,
    FOREIGN KEY(id_salle) REFERENCES salle(id_salle)
);

CREATE TABLE source (
    id_source VARCHAR(100) PRIMARY KEY,
    capacite DOUBLE PRECISION,
    unite VARCHAR(5),
    typesource INTEGER,
    pourcentage DOUBLE PRECISION
);

CREATE TABLE departement_source (
    id_departement VARCHAR(100),
    id_source VARCHAR(100),
    FOREIGN KEY(id_departement) REFERENCES departement(id_departement),
    FOREIGN KEY(id_source) REFERENCES source(id_source)
);

CREATE TABLE luminiosite (
    dateheure TIMESTAMP,
    niveau INTEGER
);

CREATE TABLE departement_coupure (
    id_departement VARCHAR(100),
    dateheure_coupure TIMESTAMP,
    FOREIGN KEY(id_departement) REFERENCES departement(id_departement)
);