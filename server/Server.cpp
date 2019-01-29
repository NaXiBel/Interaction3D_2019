#include "stdafx.h"
#include "Server.h"
#include <iostream>
#include <exception>

//using namespace std;

// DWORD WINAPI Start(void* data) {
// 	return NULL;
// }

std::map<unsigned int, SOCKET> Server::sessions;
unsigned int Server::nbUser;
unsigned int Server::tokenId;
BSpline Server::spline;
std::vector<float> Server::tabUserX;
std::vector<float> Server::tabUserY;
std::vector<float> Server::tabUserZ;


Server::Server(const int & _port)
{

	#ifdef _WIN32
		WSAStartup(MAKEWORD(2, 0), &WSAData);
	#endif

	port = _port;
	std::cout << "Démarrage du serveur." << std::endl;

	serverSocket = INVALID_SOCKET;
	SOCKET userSocket = INVALID_SOCKET;

	serverInfo.sin_family = AF_INET;
	serverInfo.sin_port = htons(port);
	serverInfo.sin_addr.s_addr = htonl(INADDR_ANY);
	serverSocket = socket(AF_INET, SOCK_STREAM, 0);
	::bind(serverSocket, (SOCKADDR *) &serverInfo, sizeof(serverInfo));
	listen(serverSocket, 0);
	std::cout << "Serveur démarré sur le port " << port << "." << std::endl;
	nbUser = 0;
	tokenId = 1;
	bool isVisited = false;
	try {
		do {
			fd_set readSet;
			FD_ZERO(&readSet);
			FD_SET(serverSocket, &readSet);
			timeval timeout = { 0 };
			int selectReady = select(serverSocket + 1, &readSet, nullptr, nullptr, &timeout);
			//ready to pair
			if (selectReady > 0) {
				SOCKET userSocket = accept(serverSocket, NULL, NULL);
				if (userSocket != INVALID_SOCKET) {
					// On ajoute l'utilisateur à la liste des connectés.
					nbUser++;
					sessions.insert(std::pair<unsigned int, SOCKET>(nbUser, userSocket));
					tabUserX.push_back(0.0f);
					tabUserY.push_back(0.0f);
					tabUserZ.push_back(0.0f);
					std::thread userThread = std::thread(listenUser, nbUser);
					userThread.detach();
					isVisited = true;
				} else {
					std::cout << "Connexion refusée." << std::endl;
				}
			}
		} while (!sessions.empty() || !isVisited);
		std::cout << "Plus personne n'est connecté." << std::endl;

		#ifdef _WIN32
			WSACleanup();
		#endif
		closesocket(serverSocket);
		std::cout << "Serveur arrêté." << std::endl;
	} catch (std::exception& e) {
		std::cout << e.what() << std::endl;
	}
	
}

int main() {
	Server serveur = Server(25128);
	return(0);
}

void Server::listenUser(unsigned int _id)
{
	char buffer[BUFLEN];
	int r = 0;
	SOCKET _socket = sessions[_id];
	std::stringstream data;
	data << "0;" << _id << ";" << tabUserX[_id] << ";" << tabUserY[_id] << ";" <<tabUserZ[_id] << ";";
	for (std::map<unsigned int, SOCKET>::iterator it = sessions.begin(); it != sessions.end(); ++it)
	{
		data << it->first;
		std::map<unsigned int, SOCKET>::iterator it2 = it;
		if (++it2 != sessions.end())data << ",";
		else data << std::endl;
	}
	std::cout << data.str();
	sendAll(data.str());
	while((r = recv(_socket, buffer, BUFLEN - 1, 0)) != 0) {
		std::string tmp = std::string(buffer, r);
		std::string s = tmp.substr(0, tmp.find('\n'));
		std::cout << "Reçu " << s << " de " << _id << "." << std::endl;
		std::vector<std::string> v;
		parseArgs(s, v);
		if (v.empty() || v[0].empty()) {
			std::cout << "pas d'arguments reçus." << std::endl;
		} else if(v[0] == "1") {
			// Demande du token.
			if(tokenId == 0) {
				tokenId = _id;
				std::cout << "Le token a été donné à l'utilisateur " << _id << std::endl;
			} else {
				std::cout << "Le token est déjà pris par " << tokenId << "." << std::endl;
			}
			sendAll(getSummary());
		} else if (v[0] == "2") {
			// Libération du token.
			releaseToken(_id);
		} else if (v[0] == "3") {
			// Set/update BSpline.
			if(atoi(v[1].c_str()) == tokenId) {
				int size = atoi(v[2].c_str());
				spline.tailleTab = size;
				std::vector<std::string> stabX(0);
				parseTab(v[3], stabX);
				std::vector<std::string> stabY(0);
				parseTab(v[4], stabY);
				std::vector<std::string> stabZ(0);
				parseTab(v[5], stabZ);
				spline.tabX = std::vector<float>(size);
				spline.tabY = std::vector<float>(size);
				spline.tabZ = std::vector<float>(size);
				for(int i = 0; i < size; i++) {
					std::stringstream tmp(stabX[i]);
					float tmpf;
					tmp >> tmpf;
					std::cout << "val x :" << stabX[i] << std::endl << std::flush;
					spline.tabX[i] = tmpf;
					spline.tabY[i] = std::atof(stabY[i].c_str());
					spline.tabZ[i] = std::atof(stabZ[i].c_str());
				}
				sendAll(getSummary());
			} else {
				std::cout << "L'utilisateur " << _id << " n'est pas autorisé à modifier la BSpline." << std::endl << std::flush;
			}
		} else if (v[0] == "4") {
			// Set/update position utilisateur.
			tabUserX[_id-1] = std::atof(v[1].c_str());
			tabUserY[_id-1] = std::atof(v[2].c_str());
			tabUserZ[_id-1] = std::atof(v[3].c_str());
			sendAll(getSummary());
			std::cout << "L'utilisateur " << _id << " se trouve désormais en " << tabUserX[_id-1] << "," << tabUserY[_id-1] << "," << tabUserZ[_id-1] <<"." << std::endl;
		} else if (v[0] == "5") {
			// Demande d'infos.
			sendUser(_id, getSummary());
		} else {
			std::cout << "Code d'opération inconnu." << std::endl;
		}
	}
	Server::disconnectUser(_id);
}

void Server::disconnectUser(unsigned int _id)
{
	SOCKET userSocket = sessions[_id];
	sessions.erase(_id);
	closesocket(userSocket);
	std::cout << "Clients connectés: " << sessions.size() <<std::endl;
	releaseToken(_id);
	std::stringstream buffer;
	buffer << "2;" << _id << ";" << std::endl;
	std::cout << "Client " << _id << " déconnecté." << std::endl;
	sendAll(buffer.str());
}

void Server::sendUser(unsigned int _id, std::string buffer)
{
	if(sessions[_id] != INVALID_SOCKET){
		send(sessions[_id], buffer.c_str(), buffer.length(), 0);
	}
}

void Server::releaseToken(unsigned int _id)
{
	if(tokenId == _id) {
		tokenId = 0;
		std::cout << "Le token est désormais libre." << std::endl;
		sendAll(getSummary());
	}
}

void Server::sendAll(std::string buffer)
{
	// Envoie un paquet à tous les utilisateurs connectés.
	std::map<unsigned int, SOCKET>::iterator it;
	for (it = sessions.begin(); it != sessions.end(); it++)
	{
    	sendUser(it->first, buffer);
	}
}

std::string Server::getSummary(void)
{
	// Génère un paquet qui contient l'intégralité des informations:
	// - Position des utilisateurs,
	// - Position des points de la spline,
	// - ID de l'utilisateur possédant le token.
	std::stringstream data;
	data << "5;" << nbUser << ";" << spline.tailleTab;
	data << ";";
	if(nbUser > 0) {
		data << tabUserX[0];
		for(unsigned i = 1; i < nbUser; i++) {
			data << "," << tabUserX[i];
		}
	}
	data << ";";
	if(nbUser > 0) {
		data << tabUserY[0];
		for(unsigned i = 1; i < nbUser; i++) {
			data << "," << tabUserY[i];
		}
	}
	data << ";";
	if(nbUser > 0) {
		data << tabUserZ[0];
		for(unsigned i = 1; i < nbUser; i++) {
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
	data << ";" << tokenId << std::endl;
	return data.str();
}

void Server::parseArgs(std::string _s, std::vector<std::string> &_v)
{
	int i = 0;
	std::string acc = "";
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

void Server::parseTab(std::string _s, std::vector<std::string> &_v)
{
	int i = 0;
	std::string acc = "";
	char c;
	std::cout << "VEC{"<<_s << "}"<< std::endl;
	std::cout << _v.size() << std::endl; 
	while((i < _s.length()) && (i < BUFLEN)) {
		c = _s[i];
		if((c == '\n') || (c == '\0')) {
			if(!acc.empty()) {
				std::cout << "cc retour" << std::endl;
				_v.push_back(acc);
			}
			break;
		} else if((c == ',')) {
			if(!acc.empty()) {
				std::cout << "cc virgule" << std::endl;
				_v.push_back(acc);
				acc.clear();
			}
		} else {
			std::cout << "cc accu" << std::endl;
			acc = acc + c;
		}
		i++;
	}
	if(!acc.empty()) {
		_v.push_back(acc);
	}
	std::stringstream ss;
	for(size_t i = 0; i < _v.size(); ++i)
	{
		if(i != 0)
		ss << ",";
		ss << _v[i];
	}
	std::string s = ss.str();
	std::cout << "stab[" << s <<"]" <<std::endl;
}
