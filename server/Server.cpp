#include "Server.h"
#include <iostream>

using namespace std;

// DWORD WINAPI Start(void* data) {
// 	return NULL;
// }

map<unsigned int, SOCKET> Server::sessions;
unsigned int Server::nbUser;
unsigned int Server::tokenId;
BSpline Server::spline;
vector<float> Server::tabUserX;
vector<float> Server::tabUserY;
vector<float> Server::tabUserZ;


Server::Server(const int & _port)
{

	#ifdef __WIN32__
		WSAStartup(MAKEWORD(2, 0), &WSAData);
	#endif

	port = _port;
	cout << "Démarrage du serveur." << endl;

	serverSocket = INVALID_SOCKET;
	SOCKET userSocket = INVALID_SOCKET;

	serverInfo.sin_family = AF_INET;
	serverInfo.sin_port = htons(port);
	serverInfo.sin_addr.s_addr = htonl(INADDR_ANY);
	serverSocket = socket(AF_INET, SOCK_STREAM, 0);
	bind(serverSocket, (SOCKADDR *) &serverInfo, sizeof(serverInfo));
	listen(serverSocket, 0);
	cout << "Serveur démarré sur le port " << port << "." << endl;
	nbUser = 0;
	tokenId = 0;

	do {
		SOCKET userSocket = accept(serverSocket, NULL, NULL);
		if (userSocket != INVALID_SOCKET) {
			// On ajoute l'utilisateur à la liste des connectés.
			nbUser++;
			sessions.insert(pair<unsigned int, SOCKET>(nbUser, userSocket));
			tabUserX.push_back(0);
			tabUserY.push_back(0);
			tabUserZ.push_back(0);
			thread userThread = thread(listenUser, nbUser);
			userThread.detach();
		} else {
			cout << "Connexion refusée." << endl;
		}
	} while(!sessions.empty());
	cout << "Plus personne n'est connecté." << endl;

	#ifdef __WIN32__
		WSACleanup();
	#endif
	closesocket(serverSocket);
	cout << "Serveur arrêté." << endl;
}

int main() {
	Server serveur = Server(5000);
	return(0);
}

void Server::listenUser(unsigned int _id)
{
	char buffer[BUFLEN];
	int r = 0;
	SOCKET _socket = sessions[_id];
	stringstream data;
	data << "0;" << _id << ";" << tabUserX[_id] << "," << tabUserY[_id] << "," <<tabUserZ[_id] << endl;
	cout << data.str();
	sendAll(data.str());
	while((r = recv(_socket, buffer, BUFLEN - 1, 0)) != 0) {
		string tmp = string(buffer, r);
		string s = tmp.substr(0, tmp.find('\n'));
		cout << "Reçu " << s << " de " << _id << "." << endl;
		vector<string> v;
		parseArgs(s, v);
		if (v.empty() || v[0].empty()) {
			cout << "pas d'arguments reçus." << endl;
		} else if(v[0] == "1") {
			// Demande du token.
			if(tokenId == 0) {
				tokenId = _id;
				cout << "Le token a été donné à l'utilisateur " << _id << endl;
			} else {
				cout << "Le token est déjà pris par " << tokenId << "." << endl;
			}
			sendAll(getSummary());
		} else if (v[0] == "2") {
			// Libération du token.
			releaseToken(_id);
		} else if (v[0] == "3") {
			// Set/update BSpline.
			if(atoi(v[1].c_str()) == tokenId) {
				int size = atoi(v[2].c_str());
				vector<string> stabX(size);
				parseTab(v[3], stabX);
				vector<string> stabY(size);
				parseTab(v[4], stabY);
				vector<string> stabZ(size);
				parseTab(v[5], stabZ);
				spline.tabX = vector<float>(size);
				spline.tabY = vector<float>(size);
				spline.tabZ = vector<float>(size);
				for(int i = 0; i < size; i++) {
					spline.tabX[i] = atof(stabX[i].c_str());
					spline.tabY[i] = atof(stabY[i].c_str());
					spline.tabZ[i] = atof(stabZ[i].c_str());
				}
				sendAll(getSummary());
			} else {
				cout << "L'utilisateur " << _id << " n'est pas autorisé à modifier la BSpline." << endl;
			}
		} else if (v[0] == "4") {
			// Set/update position utilisateur.
			tabUserX[_id-1] = atof(v[1].c_str());
			tabUserY[_id-1] = atof(v[2].c_str());
			tabUserZ[_id-1] = atof(v[3].c_str());
			sendAll(getSummary());
			cout << "L'utilisateur " << _id << " se trouve désormais en " << tabUserX[_id] << "," << tabUserY[_id] << "," << tabUserZ[_id] <<"." << endl;
		} else if (v[0] == "5") {
			// Demande d'infos.
			sendUser(_id, getSummary());
		} else {
			cout << "Code d'opération inconnu." << endl;
		}
	}
	Server::disconnectUser(_id);
}

void Server::disconnectUser(unsigned int _id)
{
	SOCKET userSocket = sessions[_id];
	sessions.erase(_id);
	close(userSocket);
	cout << "Clients connectés: " << sessions.size() << endl;
	releaseToken(_id);
	stringstream buffer;
	buffer << "Client " << _id << " déconnecté." << endl;
	cout << buffer.str();
	sendAll(buffer.str());
}

void Server::sendUser(unsigned int _id, string buffer)
{
	if(sessions[_id] != INVALID_SOCKET){
		send(sessions[_id], buffer.c_str(), buffer.length(), 0);
	}
}

void Server::releaseToken(unsigned int _id)
{
	if(tokenId == _id) {
		tokenId = 0;
		cout << "Le token est désormais libre." << endl;
	}
	sendAll(getSummary());
}

void Server::sendAll(string buffer)
{
	// Envoie un paquet à tous les utilisateurs connectés.
	map<unsigned int, SOCKET>::iterator it;
	for (it = sessions.begin(); it != sessions.end(); it++)
	{
    	sendUser(it->first, buffer);
	}
}

string Server::getSummary(void)
{
	// Génère un paquet qui contient l'intégralité des informations:
	// - Position des utilisateurs,
	// - Position des points de la spline,
	// - ID de l'utilisateur possédant le token.
	stringstream data;
	data << "5;" << nbUser << ";" << spline.tailleTab;
	data << ";";
	if(nbUser > 0) {
		data << tabUserX[0];
		for(int i = 1; i < nbUser; i++) {
			data << "," << tabUserX[i];
		}
	}
	data << ";";
	if(nbUser > 0) {
		data << tabUserY[0];
		for(int i = 1; i < nbUser; i++) {
			data << "," << tabUserY[i];
		}
	}
	data << ";";
	if(nbUser > 0) {
		data << tabUserZ[0];
		for(int i = 1; i < nbUser; i++) {
			data << "," << tabUserZ[i];
		}
	}
	data << ";";
	if(spline.tailleTab > 0) {
		data << spline.tabX[0];
		for(int i = 1; i < spline.tailleTab; i++) {
			data << "," << spline.tabX[i];
		}
	}
	data << ";";
	if(spline.tailleTab > 0) {
		data << spline.tabY[0];
		for(int i = 1; i < spline.tailleTab; i++) {
			data << "," << spline.tabY[i];
		}
	}
	data << ";";
	if(spline.tailleTab > 0) {
		data << spline.tabZ[0];
		for(int i = 1; i < spline.tailleTab; i++) {
			data << "," << spline.tabZ[i];
		}
	}
	data << ";" << tokenId << endl;
	return data.str();
}

void Server::parseArgs(string _s, vector<string> &_v)
{
	int i = 0;
	string acc = "";
	char c;
	while((i < _s.length()) && (i < BUFLEN)) {
		c = _s[i];
		if((c == '\n') || (c == '\0')) {
			if(!acc.empty()) {
				_v.push_back(acc);
			}
			break;
		} else if((c == ';')) {
			if(!acc.empty()) {
				_v.push_back(acc);
				acc.clear();
			}
		} else {
			acc = acc + c;
		}
		i++;
	}
	if(!acc.empty()) {
		_v.push_back(acc);
	}
}

void Server::parseTab(string _s, vector<string> &_v)
{
	int i = 0;
	string acc = "";
	char c;
	while((i < _s.length()) && (i < BUFLEN)) {
		c = _s[i];
		if((c == '\n') || (c == '\0')) {
			if(!acc.empty()) {
				_v.push_back(acc);
			}
			break;
		} else if((c == ',')) {
			if(!acc.empty()) {
				_v.push_back(acc);
				acc.clear();
			}
		} else {
			acc = acc + c;
		}
		i++;
	}
	if(!acc.empty()) {
		_v.push_back(acc);
	}
}
