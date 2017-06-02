debug:
	xbuild ./src/Contest.sln /p:configuration=DEBUG /t:rebuild

release:
	xbuild ./src/Contest.sln /p:configuration=RELEASE /t:rebuild
