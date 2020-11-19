DROP DATABASE IF EXISTS datos;
CREATE DATABASE datos;

USE datos;

CREATE TABLE Jugador (
        id INTEGER NOT NULL,
	nom VARCHAR(60) NOT NULL,
	pword VARCHAR(60) NOT NULL,
	ganadas INTEGER NOT NULL,
	jugadas INTEGER NOT NULL,
        PRIMARY KEY(id)
 

	)ENGINE = InnoDB;
	
CREATE TABLE Partida (
	id INTEGER NOT NULL,
        fecha VARCHAR(60) NOT NULL,
        inicio VARCHAR(60) NOT NULL,
	duracion VARCHAR (60) NOT NULL,
	ganador VARCHAR(60) NOT NULL,
	PRIMARY KEY(Id)
	)ENGINE = InnoDB;
	
CREATE TABLE Relacion (
	Jugadores  INTEGER NOT NULL,
	Partidas INTEGER NOT NULL,
								
	FOREIGN KEY (Jugadores) REFERENCES Jugador(id),
	FOREIGN KEY (Partidas) REFERENCES Partida(id)

	)ENGINE = InnoDB;

INSERT INTO Jugador VALUES (1,'david', 'e1', 1, 4);
INSERT INTO Jugador VALUES (2,'polet', 'e2', 1, 4);
INSERT INTO Jugador VALUES (3,'cristina', 'e3', 2, 4);

INSERT INTO Partida VALUES ( 1, '4/10' , '10:30' , ' 00:60' , 'polet');
INSERT INTO Partida VALUES ( 2, '4/10' , '10:30' , ' 00:40' , 'david');
INSERT INTO Partida VALUES ( 3, '4/10' , '10:30' , ' 00:50' , 'polet');
INSERT INTO Partida VALUES ( 4, '4/10' , '10:30' , ' 00:30' , 'polet');

INSERT INTO Relacion VALUES (1,1);
INSERT INTO Relacion VALUES (3,2);
INSERT INTO Relacion VALUES (2,3);
INSERT INTO Relacion VALUES (3,4);

SELECT Jugador.ganadas FROM Jugador WHERE Jugador.nom ='david';

