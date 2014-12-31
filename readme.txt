Use Visual Studio 2012 Express or 2013 Community to open this solution.
2013 Community can be downloaded from here: http://go.microsoft.com/fwlink/?LinkId=517284 

Install the Azure SDK, found here: http://azure.microsoft.com/en-us/downloads/ 
The solution was created with version 2.5. 

Some nuget packages are installed so you will have to enable nuget restoration and download those before you can compile successfully. 

To enable multiple instances of your roles to be started at the same time and load balanced via the emulated app fabric, (almost) follow the instructions here http://msdn.microsoft.com/en-us/library/azure/dn339018.aspx and "Use Full Emulator" instead. You can right click on the emulator icon in the start tray to see how many instances are running and the port for each. 

For development, we will use our local storage emulators. Once we have an "idea system" ready for deployment, I will sign us up for storage since it's 7 cents a GB and $1 will get us approx 26.3 million IOs.