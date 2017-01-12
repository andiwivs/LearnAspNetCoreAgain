
/***************************************************
 * Local (dev instance) Db setup
 ***************************************************/

From Package Manager Console / Powershell, navigate to site root folder:
cd .\TheWorld

Then run EF migrations update command:
dotnet ef database update

Note: in Visual Studio use View > SQL Server Object Explorer to access local dev db instance