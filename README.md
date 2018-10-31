# MovieRental

A code assessment

-----

This project is best run from Visual Studio 2017, using SQL Express and IIS Express.

Instructions:
* Download code, open in Visual Studio
* From Package Manager, select the MovieRental.Entity project, and run `Update-Database` to run mogrations and seed the first user.
* Run project
* Use cURL or any rest client to run methods

-----

## Commands
### Account

Create

* CreateAccount `curl -d '{"username":"next","password":"user"}' -H "Content-Type: application/json" -X POST "http://localhost:50137/account/"`
* Login: `curl -d "username=joe&password=user&grant_type=password" -X POST http://localhost:50137/token`
  - Get the access token from this for all other commands
* GetAccount: Two options
  - Get own info: `curl -H "authorization: Bearer {token}" "http://localhost:50137/account/"`
  - Get Other user (admin only): `curl -H "authorization: Bearer {token}" "http://localhost:50137/account/2/"`
* UpdateAccount: Optional id as above: `curl -d '{"username":"next", "password":"user", "userrole": "Admin"}' -H "Content-Type: application/json" -X PUT -H "authorization: Bearer {token}" "http://localhost:50137/account/2/"`
* DeleteAccount (deactivate): Optional id as above: `curl -X DELETE -H "authorization: Bearer {token}" "http://localhost:50137/account/2/"`
* RentalHistory: Optional id as above: `curl -H "authorization: Bearer {token}" "http://localhost:50137/account/1/history"`
