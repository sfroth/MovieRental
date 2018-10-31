# MovieRental

A code assessment challenge: Build an API that will support a movie catalog and rental service (similar to Redbox)

-----

This project is best run from Visual Studio 2017, using SQL Express and IIS Express.

Instructions:
* Download code, open in Visual Studio
* From Package Manager, select the MovieRental.Entity project, and run `Update-Database` to run migrations and seed the first user.
* Run project
* Use cURL or any rest client to run methods

-----

## Commands
### Account

* CreateAccount `curl -d '{"username":"next","password":"user"}' -H "Content-Type: application/json" -X POST "http://localhost:50137/account/"`
* Login: `curl -d "username=joe&password=user&grant_type=password" -X POST http://localhost:50137/token`
  - Get the access token from this for all other commands
* GetAccount: Two options
  - Get own info: `curl -H "authorization: Bearer {token}" "http://localhost:50137/account/"`
  - Get Other user (admin only): `curl -H "authorization: Bearer {token}" "http://localhost:50137/account/2/"`
* UpdateAccount: Optional id as above: `curl -d '{"username":"next", "password":"user", "userrole": "Admin"}' -H "Content-Type: application/json" -X PUT -H "authorization: Bearer {token}" "http://localhost:50137/account/2/"`
* DeleteAccount (deactivate): Optional id as above: `curl -X DELETE -H "authorization: Bearer {token}" "http://localhost:50137/account/2/"`
* RentalHistory: Optional id as above: `curl -H "authorization: Bearer {token}" "http://localhost:50137/account/1/history/"`

### Movie

* CreateMovie: `curl -d '{"title":"Serenity","releasedate":"2005-09-01"}' -H "Content-Type: application/json" -H "authorization: Bearer {token}" -X POST "http://localhost:50137/movie/"`
* UpdateMovie: `curl -d '{"title":"Serenity","releasedate":"2005-09-30"}' -H "Content-Type: application/json" -H "authorization: Bearer {token}" -X PUT "http://localhost:50137/movie/1/"`
* GetMovie: `curl "http://localhost:50137/movie/1/"`
* SearchMovies: `curl -d '{"SearchTerm":"ere","releasefrom":"2005-01-01", "releaseto": "2006-01-01"}' -H "Content-Type: application/json" -X POST "http://localhost:50137/movies/"`

### Kiosk

* CreateKiosk: `curl -d '{"name":"Vons","Streetaddress1":"123 Main St","City":"Here","Country":"US","PostalCode":"91701"}' -H "Content-Type: application/json" -H "authorization: Bearer {token}" -X POST "http://localhost:50137/kiosk/"`
* UpdateKiosk: `curl -d '{"name":"Vons","Streetaddress1":"123 Main St","City":"Here","Country":"US","PostalCode":"91737"}' -H "Content-Type: application/json" -H "authorization: Bearer {token}" -X PUT "http://localhost:50137/kiosk/1/"`
* GetKiosk: `curl -H "authorization: Bearer {token}" "http://localhost:50137/kiosk/1/"`
* DeleteKiosk: `curl -H "authorization: Bearer {token}" -X DELETE "http://localhost:50137/kiosk/1/"`
* GetKiosksNear: `curl -H "authorization: Bearer {token}" "http://localhost:50137/kiosks/?location.PostalCode=91737&distance=10000"`
* AddMoviesToKiosk: `curl -d '[{"MovieID":1,"stock":5}]' -H "Content-Type: application/json" -H "authorization: Bearer {token}" -X POST "http://localhost:50137/kiosk/1/addmovies"`
* RemoveMoviesFromKiosk: `curl -d '[{"MovieID":1,"stock":1}]' -H "Content-Type: application/json" -H "authorization: Bearer {token}" -X POST "http://localhost:50137/kiosk/1/removemovies"`
* GetMoviesAtKiosk: `curl -H "authorization: Bearer {token}" "http://localhost:50137/kiosk/1/movies/"`
* RentMovie: `curl -d '' -H "authorization: Bearer {token}" -X POST "http://localhost:50137/kiosk/1/rent/1/"`
* ReturnMovie: `curl -d '' -H "authorization: Bearer {token}" -X POST "http://localhost:50137/kiosk/1/return/1/"`
