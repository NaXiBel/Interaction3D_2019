all: Server
	g++ Server.o -o server.exe -lpthread

Server: Server.cpp Server.h
	g++ -c Server.cpp -std=c++17

User: User.h User.cpp
	g++ -c User.h -std=c++17

clean:
	rm *.o

cleanall: clean
	rm server.exe
