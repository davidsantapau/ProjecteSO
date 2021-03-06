#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>


//-std=c99 `mysql_config --cflags --libs`
// ESTRUCTURAS
int cont= 0;
int sockets[100];

typedef struct{
	char orden[20];
}TOrden;
typedef struct{
	int num;
	TOrden servicios[100];
}TServicios;
TServicios lista_servicios;

typedef struct{
	char jugador[20];
	int socket;
}TUser;
typedef struct{
	int num;
	TUser usuarios[100];
}TConectados;
typedef struct{
	char nom[20];
	char mensaje [100]
}Tmensaje;
typedef struct{
	int num;
	Tmensaje msg[100];
}Tmensajes;
typedef struct{
	char jugador[20];
	char jugador2 [100]
}TPartida;
typedef struct{
	int num;
	TPartida partidas[100];
}TPartidas;
TPartidas lista_partidas;
Tmensajes lista_mensaje;
TConectados lista_conectados;
int isocket;
pthread_mutex_t mutex /*= PTHREAD_MUTEX_INITIALIZER*/;

int PonLista(TConectados *l, char jugador[20],int socket) // Guarda el usuario en la lista de conectados
{
	if (l->num<100)
	{
		strcpy(l->usuarios[l->num].jugador,jugador);
		l->usuarios[l->num].socket=socket;
		l->num++;
		return 0;
	}
	else
		return -1;
}
int Msg(Tmensajes *l, char msg[100], char lnom [20]) // Guarda el mensaje en la lista de mensajes
{
	if (l->num<100)
	{
		strcpy(l->msg[l->num].mensaje,msg);
		strcpy(l->msg[l->num].nom,lnom);
		
		l->num++;
		return 0;
	}
	else
		return -1;
}
int EliminarLista(TConectados *l, int socket) // Eliminar el usuario que se ha desconectado de la lista de conectados
{
	int encontrado=0;
	int i=0;
	while((i<l->num) && (encontrado==0))
	{
		if(l->usuarios[i].socket==socket)
			encontrado=1;
		else
			i++;
	}
	if (!encontrado)
		return -1;
	else
	{
		while(i<l->num-1)
		{
			l->usuarios[i]=l->usuarios[i+1];
			i++;
		}
		l->num=l->num-1;
		return 0;
	}
}
int UsuarioExistente(char nom[20]) // Busca si el usuario que intenta registrarse ya existe retorna 0  si no hay usuarios con ese nombre y 1 en caso contrario
{
	MYSQL *conn;
	int err;
	//++++++++++++++++++++++++++++++++++Almacenar consulta
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	//++++++++++++++++++++++++++++++++++Establecemos conexion
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//inicializar la conexion
	conn = mysql_real_connect (conn, "shiva2.upc.es","root", "mysql", "T6_BBDDJuego",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//++++++++++++++++++++++++++++++++++++++++++CONSULTA
	sprintf(consulta, "SELECT Jugador.nom FROM Jugador WHERE Jugador.nom = '%s'\n", nom);
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	if (row == NULL) //no hay usuarios con ese nombre
		return 0;
	else            //nombre ya existe
		return -1;
	
}
int Registro(char nom[20], char pword[20], int id) // Guarda un nuevo usuario en la base de datos. 1 si hay errores sino 0
{
	MYSQL *conn;
	int err;
	//++++++++++++++++++++++++++++++++++Almacenar consulta
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	//++++++++++++++++++++++++++++++++++Establecemos conexion
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//inicializar la conexion
	conn = mysql_real_connect (conn,"shiva2.upc.es","root", "mysql", "T6_BBDDJuego",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//++++++++++++++++++++++++++++++++++++++++++CONSULTA
	sprintf(consulta,"INSERT INTO Jugador VALUES (NULL, '%s', '%s',0,0);",nom, pword);
	printf("%s\n",consulta);
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	
	mysql_close (conn);
	return 0;
}
int Login(char lnom[20], char lpword[20]) // Mira si la persona la persona que se intenta loguear existe y si su contrase�a es correcta. Retorna 1 si hay errores, 0 si esta todo correcto, -1 si la persona no existe o la contrase�a es incorrecta
{
	MYSQL *conn;
	int err;
	//++++++++++++++++++++++++++++++++++Almacenar consulta
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	//++++++++++++++++++++++++++++++++++Establecemos conexion
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//inicializar la conexion
	conn = mysql_real_connect (conn,"shiva2.upc.es","root", "mysql", "T6_BBDDJuego",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//++++++++++++++++++++++++++++++++++++++++++CONSULTA
	sprintf(consulta, "SELECT nom FROM Jugador WHERE Jugador.nom='%s' AND Jugador.pword='%s' ",lnom, lpword);
	printf ("%s\n",consulta); 
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	
	resultado = mysql_store_result (conn);
	
	row = mysql_fetch_row (resultado);
	if (row == NULL) 
		return 0;
	else            
		return -1;
		
}

int Consulta1(char C1nom[200]) //dame campeon
{
	printf ("ff\n");
	MYSQL *conn;
	int err;
	//++++++++++++++++++++++++++++++++++Almacenar consulta
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	//++++++++++++++++++++++++++++++++++Establecemos conexion
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//inicializar la conexion
	conn = mysql_real_connect (conn,"shiva2.upc.es","root", "mysql", "T6_BBDDJuego",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//++++++++++++++++++++++++++++++++++++++++++CONSULTA
	sprintf(consulta, "SELECT Jugador.nom FROM Jugador WHERE Jugador.ganadas = (SELECT MAX(Jugador.ganadas) FROM Jugador);");
	err=mysql_query (conn, consulta);
	
	
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
		
		resultado = mysql_store_result (conn);
	
		
	/*char campeon[20] = mysql_store_result(resultado);*/
	row = mysql_fetch_row (resultado);
	//al obtener la consulta
	char respuesta [512];

	
	if (row == NULL)
	{		sprintf (C1nom,"No se han obtenido datos en la consulta\n");
	
	    return 1;
	}
	else
	{	while (row !=NULL)
	    {
			sprintf (C1nom,"%s\n", row[0]);
			// obtenemos la siguiente fila
			row = mysql_fetch_row (resultado);
	    }
	  return 0;
		
	
	}
	printf ("%s\n",C1nom);
}
int Consulta2(char C2nom[20], int tiempo)//duracion de la partida mas larga
{
	MYSQL *conn;
	int err;
	//++++++++++++++++++++++++++++++++++Almacenar consulta
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	//++++++++++++++++++++++++++++++++++Establecemos conexion
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf("fa\n");
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//inicializar la conexion
	conn = mysql_real_connect (conn,"shiva2.upc.es","root", "mysql", "T6_BBDDJuego",0, NULL, 0);
	if (conn==NULL) {
		printf("fb\n");
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//++++++++++++++++++++++++++++++++++++++++++CONSULTA
	//sprintf(consulta, "SELECT MAX(Partida.duracion) FROM Partida, Relacion, Jugador WHERE Jugador.nom= '%s' AND Jugador.id= Relacion.id AND Relacion.id =Partida.id\n", C2nom);
	sprintf (  consulta, "SELECT MAX(Partida.duracion) FROM Partida");
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf("fc\n");
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (-1);
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	char respuesta [200];	
	if (row == NULL)
		sprintf (respuesta,"No se han obtenido datos en la consulta\n");
	else
	{	while (row !=NULL) 
	       {
			// la columna 0 contiene el nombre del jugador
			sprintf (respuesta,"%s\n", row[0]);
			// obtenemos la siguiente fila
			row = mysql_fetch_row (resultado);
	       }
		
	    tiempo = atoi (respuesta);
		return tiempo;
	}
	/*strcpy ( tiempo, respuesta);*/ 
}
int Consulta3( char nom[20])// Partidas ganadas por un jugador
{
	MYSQL *conn;
	int err;
	//++++++++++++++++++++++++++++++++++Almacenar consulta
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	//++++++++++++++++++++++++++++++++++Establecemos conexion
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (-1);
	}
	//inicializar la conexion
	conn = mysql_real_connect (conn, "shiva2.upc.es","root", "mysql", "T6_BBDDJuego",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (-1);
	}
	//++++++++++++++++++++++++++++++++++++++++++CONSULTA
	sprintf(consulta,  "SELECT Jugador.ganadas FROM Jugador WHERE Jugador.nom = '%s'",nom);
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (-1);
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	char respuesta [30];
	if (row == NULL)
	{	
	    sprintf (respuesta,"No se han obtenido datos en la consulta\n");
	    return (0);
	}
	else
	{	while (row !=NULL) 
	      {
		    // la columna 0 contiene el nombre del jugador
		     sprintf (respuesta,"%s\n", row[0]);
		    // obtenemos la siguiente fila
		     row = mysql_fetch_row (resultado);
	      }
	int ganadas= atoi (respuesta);
	return ganadas;
	}
}

int BBDD ()
{
	//Conector para acceder al servidor de bases de datos 
	MYSQL *conn;
	int err;
	//Creamos una conexion al servidor MYSQL 
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		
		return (1);
	}
	//inicializar la conexion, indicando nuestras claves de acceso
	// al servidor de bases de datos (user,pass)
	conn = mysql_real_connect (conn,"shiva2.upc.es","root", "mysql",  NULL, 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	
	// ahora vamos a crear la base de datos, que se llamara mydatabase
	// primero la borramos si es que ya existe (quizas porque hemos
	// hecho pruebas anteriormente
	mysql_query(conn, "drop database if exists mydatabase;"); 
	err=mysql_query(conn, "create database mydatabase;");
	if (err!=0) {
		printf ("Error al crear la base de datos %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//indicamos la base de datos con la que queremos trabajar 
	err=mysql_query(conn, "use mydatabase;");
	if (err!=0)
	{
		printf ("Error al crear la base de datos %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	// creamos la tabla que define la entidad persona: 
	// 	un DNI (clave principal), nombre y edad 
	err=mysql_query(conn,
					"CREATE TABLE Jugador (id integer not null primary key AUTO_INCREMENT, nom VARCHAR(60) not null , pword VARCHAR(60) not null, ganadas INTEGER not null, jugadas INTEGER not null)");
	
	if (err!=0)
	{
		printf ("Error al definir la tabla %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	err=mysql_query(conn,
					"CREATE TABLE Partida (id integer not null primary key AUTO_INCREMENT, fecha VARCHAR(60) not null, inicio VARCHAR(60) not null , duracion integer, ganador VARCHAR(60))");
	
	if (err!=0)
	{
		printf ("Error al definir la tabla %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	err=mysql_query(conn,
					"CREATE TABLE Relacion (Jugadores integer not null, Partidas integer not null, FOREIGN KEY (Jugadores) REFERENCES Jugador(id), FOREIGN KEY (Partidas) REFERENCES Partida(id))");
	
	if (err!=0)
	{
		printf ("Error al definir la tabla %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	err=mysql_query(conn,"insert into Jugador (id, nom, pword, ganadas, jugadas) values (NULL,'david','e1', 3, 5);");
	
	if (err!=0)
	{
		printf ("Error al definir la tabla %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	err=mysql_query(conn,"insert into Jugador (id, nom, pword, ganadas, jugadas) values (NULL,'polet','e2', 0, 7);");
	
	if (err!=0)
	{
		printf ("Error al definir la tabla %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	err=mysql_query(conn,"insert into Jugador (id, nom, pword, ganadas, jugadas) values (NULL,'cristina','e3', 1,8);");
	
	if (err!=0)
	{
		printf ("Error al definir la tabla %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	err=mysql_query(conn,"insert into Partida (id, fecha, inicio, duracion, ganador) values (NULL, '03/14','3:04','2','polet');");
	
	if (err!=0)
	{
		printf ("Error al definir la tabla %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}	
	err=mysql_query(conn,"insert into Partida (id, fecha, inicio, duracion, ganador) values (NULL, '03/14','3:04','5','polet');");
	
	if (err!=0)
	{
		printf ("Error al definir la tabla %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}	
	err=mysql_query(conn,"insert into Relacion (Jugadores, Partidas) values (1, 1);");
	
	if (err!=0)
	{
		printf ("Error al definir la tabla %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}	
	err=mysql_query(conn,"insert into Relacion (Jugadores, Partidas) values (2, 1);");
	
	if (err!=0)
	{
		printf ("Error al definir la tabla %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}	
	// ahora tenemos la base de datos lista en el servidor de MySQL
	// cerrar la conexion con el servidor MYSQL 
	mysql_close (conn);
}
int Companys(TConectados *l, char invitacio[20])//Buscar el socket del invitado o del rival
{
	int i=0;
	int trobat=0;
	printf("%s\n", invitacio);
	while ((i<l->num)&&(trobat==0))
	{
		
		
		printf("%s=%s? \n", l->usuarios[i].jugador, invitacio);
		if (strcmp(l->usuarios[i].jugador, invitacio) == 0)
		{
			trobat=1;
		}
		else
			i++;
	}
	if (trobat==0)
	{
		return -1;//no encontrado
	}
	else
		return l->usuarios[i].socket;//devuelve el socket del invitado
	
}

void anfitrio(TConectados *l, int socket, char anom[20]) // Busca un nombre a partir de un socket
{
	int i=0;
	int trobat=0;
	while ((i<l->num)&&(trobat==0))
	{

		if (l->usuarios[i].socket == socket)
		{
			trobat=1;
			printf("%s\n",l->usuarios[i].jugador);
			strcpy(anom, l->usuarios[i].jugador);
			
			
		}
		else
			i++;
	}
}
int guardar(int socket) //Guardar socket del que invita
{
	isocket= socket;
}
void *AtenderCliente(void *socket)
{
	
	int sock_conn;
	int ret;

	int *s;
	s = (int*) socket;
	sock_conn = *s;
	char buff[512];
	char respuesta[512];
	
	char user[20];
	int terminar = 0;
	while (terminar == 0)
	{
		// Recibimos peticion de un usuario de nombre nom
		ret=read(sock_conn,buff, sizeof(buff));
		printf ("Recibido\n");
		
		buff[ret]='\0'; //fin de string
		
		//Escribimos en consola quien se ha conectado
		printf ("El c�digo de peticion es: %s\n",buff);
		/*int i =1;*/
		
		//Servicios
		
		char *servicios = strtok(buff, "/");
		int codigo = atoi (servicios);
		int i=0;
		while ((servicios = strtok(NULL,"/")) != NULL)
		{
				//mensaje troceado
			strcpy(lista_servicios.servicios[i].orden,servicios);
			i++;
		}
		
		
		                                           	//situaciones
		if (codigo==0) //Desconectar
		{
				int socket;
				printf("Desconectar socket %d\n", sock_conn);
				socket=sock_conn;
				pthread_mutex_lock(&mutex);
				int e= EliminarLista (&lista_conectados,socket);
				pthread_mutex_unlock(&mutex);
				
				int j;
				j=0;
				strcpy(respuesta, "6/0/");
				char numero[10];
				sprintf(numero, "%d", lista_conectados.num);
				strcat(respuesta, numero);
				
				while ( j < lista_conectados.num)
				{
					strcat(respuesta, "/");
					strcat (respuesta, lista_conectados.usuarios[j].jugador);
					j = j + 1;
				}
				
				//enviar por todos los sockets q tengo conectados en ese momento
				int k;
				for(k=0; k<lista_conectados.num; k++)
					write(lista_conectados.usuarios[k].socket,respuesta, strlen(respuesta));
				
				terminar=1;
		}
			   
		if(codigo==1)
		{                       	//Registro
				
				char rnom[20];
				char rpword[20];
				
				strcpy (rnom, lista_servicios.servicios[0].orden);
				strcpy (rpword, lista_servicios.servicios[1].orden);
				int existe = UsuarioExistente(rnom);
				if(existe == 0)
				{  //no hay usuarios con ese nom
				    int r = Registro(rnom,rpword,cont);
					cont++;
					strcpy(respuesta, "1/0");
					
				}
				else //usuario exitente
				{		
					strcpy(respuesta, "1/-1");
					printf("Error Registro");
				}
				write(sock_conn,respuesta,strlen(respuesta));
				codigo= 0;
		}
		if (codigo == 2) //LOGIN
		{

				char lnom [20];
				char lpword[20];
				strcpy (lnom, lista_servicios.servicios [0].orden);
				strcpy (lpword, lista_servicios.servicios[1].orden);
				int lerr= Login(lnom, lpword);
				
				if (lerr==-1)
				{
					strcpy ( respuesta, "2/0");
					char jugador[20];
					int socket;
					strcpy(jugador, lnom);
					socket=sock_conn;
					pthread_mutex_lock(&mutex);
					write (sock_conn, respuesta, strlen(respuesta));
					cont=cont+1;
					printf("%d\n", sock_conn);
					pthread_mutex_unlock (&mutex); // Ya puedes interrumpirme
					
					int p= PonLista(&lista_conectados, jugador, socket);
					// notificar a todos los clientes conectados
					int j;
					j=0;
					strcpy(respuesta, "6/0/");
					char numero[10];
					sprintf(numero, "%d", lista_conectados.num);
					strcat(respuesta, numero);
					
					while ( j < lista_conectados.num)
					{
						strcat(respuesta, "/");
						strcat (respuesta, lista_conectados.usuarios[j].jugador);
						
						j = j + 1;
					}
					strcat(respuesta, "/");
					printf("%s\n", respuesta);
					
					//enviar por todos los sockets q tengo conectados en ese momento
					int k;
					for(k=0; k<lista_conectados.num; k++)
						write(lista_conectados.usuarios[k].socket,respuesta, strlen(respuesta));
					
					
				}
				else
				{
					
					strcpy (respuesta, "2/1");
					write (sock_conn, respuesta, strlen(respuesta));
					strcpy (respuesta, "8");
					write (sock_conn, respuesta, strlen(respuesta));
					
				}
				
						
		}
			
		if (codigo==3) //nom del campeon
		{
			char C1nom[200];
			
			int C1 = Consulta1(C1nom);
			
			if (C1==0)
			{
				sprintf (respuesta,"3/0/%s\n",C1nom);
				
			}
			else
			{
				strcpy(respuesta,"Error");
			}
			write (sock_conn,respuesta, strlen(respuesta));
		}
		if(codigo ==4) //partida mas larga
		{
			char C2nom[20];
			
			int duracion;
			strcpy (C2nom, lista_servicios.servicios[0].orden);
			int C2 = Consulta2(C2nom, duracion);
			
			
			if (C2 > 0)
			{
				sprintf (respuesta,"4/-1/%d\n", C2);						
			}
			else
			{
				strcpy(respuesta,"Error");
			}
			
			
			write (sock_conn,respuesta, strlen(respuesta));
			
		}
		if (codigo == 5) // partidas ganadas por un jugador
		{
			char C3nom[20];
			strcpy (C3nom, lista_servicios.servicios[0].orden);
			int C3 = Consulta3(C3nom);
			if (C3 >= 0)
			{
				sprintf (respuesta,"5/-1/ %d\n", C3);
				
			}
			else
			{
				strcpy(respuesta,"Error");
			}
			write (sock_conn,respuesta, strlen(respuesta));
			
		}
		if (codigo == 6)	//enviar lista de conectados
		{
			int j;
			j=0;
			strcpy(respuesta, "6/");
			char numero[10];
			sprintf(numero, "%d", lista_conectados.num);
			strcat(respuesta, numero);
			
			while ( j < lista_conectados.num)
			{
				strcat(respuesta, "/");
				strcat (respuesta, lista_conectados.usuarios[j].jugador);
				j = j + 1;
			}
			write (sock_conn,respuesta, strlen(respuesta));
			
		}
		if (codigo==7) //invitaci�n
		{
			char invitacio[20];//nombre del invitado
			guardar(sock_conn);
			strcpy(invitacio,lista_servicios.servicios[0].orden);
			printf ("%d\n", sock_conn);
			int s= Companys(&lista_conectados, invitacio);//buscamos el socket del invitado
			if (s == -1)
			{
				
				strcpy(respuesta, "7/3");//no encontrado
				write(sock_conn, respuesta, strlen(respuesta));
			}
			else
			{
				char anom[20];
				anfitrio (&lista_conectados, sock_conn, anom);
				printf("%s\n",anom);
				sprintf(respuesta, "7/2/%s/\n",lista_servicios.servicios[1].orden);//he encontrado al que invito en las lista conectados
				strcat(respuesta,anom);
				strcat(respuesta," t'ha convidat");
				write(s, respuesta, strlen(respuesta));
			}
			printf("%s\n",respuesta);
		}
		if (codigo==8)//Respuesta del invitado
		{
			
			int r = atoi (lista_servicios.servicios[0].orden);
			if (r==0)
			{
				strcpy(respuesta, "7/0");//ha aceptado la invitacion
				write( isocket  , respuesta, strlen(respuesta));
			}
			else
			{
				strcpy(respuesta, "7/1");//ha rechazado la invitacion
				printf ("%d\n", isocket);
				write(isocket , respuesta,strlen(respuesta));
			}
		}
		if (codigo==9) // movimiento con los parametros pasados por el cliente
		{
			char invitacio [20];
			strcpy( invitacio, lista_servicios.servicios[0].orden);
			int soc= Companys  (&lista_conectados, invitacio);
			printf("%d\n", soc);
			sprintf( respuesta, "9/%s/%s/%s/%s\n", lista_servicios.servicios[1].orden, lista_servicios.servicios[2].orden, lista_servicios.servicios[3].orden,lista_servicios.servicios[4].orden);
			printf("%s\n", respuesta);
			write (soc, respuesta, strlen(respuesta));
			write (soc, respuesta, strlen(respuesta));
		}
		if (codigo==10) //mensaje devuelve notificacion a todos los usuarios conectados con la lista de mensajes
		{
			char lnom [20];
			char lmsg[100];
			strcpy (lnom, lista_servicios.servicios [0].orden);
			strcpy (lmsg, lista_servicios.servicios[1].orden);
				
		    int l= Msg(&lista_mensaje, lmsg, lnom);
			int j;
			j=0;
			strcpy(respuesta, "10/");
			char numero[10];
			int r = lista_mensaje.num;
			sprintf(numero, "%d", r);
			strcat(respuesta, numero);
			printf("%s\n", respuesta);
			
			while ( j < lista_mensaje.num)
			{
				char cacho [120];
				sprintf (cacho, "%s: %s\n", lista_mensaje.msg[j].nom, lista_mensaje.msg[j].mensaje);
				strcat(respuesta, "/");
				strcat (respuesta, cacho);
					
				j = j + 1;
			}
			strcat(respuesta, "/");
			printf("%s\n", respuesta);
				
				//enviar por todos los sockets q tengo conectados en ese momento
			int k;
			for(k=0; k<lista_conectados.num; k++)
				write(lista_conectados.usuarios[k].socket,respuesta, strlen(respuesta));				
				

		}
	}
}
	



int main(int argc, char *argv[])
{
	
	int sock_conn;
	int sock_listen;
	struct sockaddr_in serv_adr;
	pthread_t thread[100];
	/*int sockets[100];*/
	
	
	// INICIALITZACIONS

    if ((sock_listen = socket(AF_INET, SOCK_STREAM,0)) < 0)
      	printf("Error creant socket");
      	// Fem el bind al port
	
	memset(&serv_adr, 0, sizeof(serv_adr));// inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	// asocia el socket a cualquiera de las IP de la m?quina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	
	// escucharemos en el port 9050
	serv_adr.sin_port = htons(50066);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind");
	//La cola de peticiones pendientes no podr? ser superior a 4
	if (listen(sock_listen, 2) < 0)
		printf("Error en el Listen");
	int i;
	int j=0;	
	int e= BBDD	();
	pthread_mutex_init(&mutex, NULL);
	// Atenderemos solo 5 peticione
	for(;;)
	{
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexion\n");
		//sock_conn es el socket que usaremos para este cliente
		sockets[j] = sock_conn;
		pthread_create ( &thread[j], NULL, AtenderCliente, &sockets[j]);
		j++;
	}
	for (i=0;i<100;i++){
		pthread_join(thread[i],NULL);
	}
		
}


