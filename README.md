## ShorterThanAverage - The Best URL Shortener in Town*!

<img src="https://i.imgur.com/RvHLUZV.png" width="250" height="350">

#### Usage:
POST endpoint (For shortening):\
/api/shorten

Parameters:\
request (required) (string) - a valid URL that the user would like shortened\
vanity (string) - vanity short code that the user would like for their URL, if not provided, a generated short code will be provided.

GET endpoint (for redirecting):\
/{short_code}\
short_code (string) the short code that the user generated in the POST endpoint.

---

How to set up and run the project:
* Download and open the project
* Set up a PostgreSQL Server
* Your table should have the following fields
* URL (text) - for the full URL
* ShortURL (text) (with a UNIQUE constraint)- for the short code
* Edit PostgreDB in appsettings.json according to your PostgreSQL setup
* Make sure you have the following packages:
	* Base62-Net
	* Microsoft.AspNetCore.OpenApi
	* Npgsql
	* Npgsql.EntityFrameworkCore.PostgreSQL
	* Swashbuckle.AspNetCore
* Project made with .Net 9.0
* Run Program.cs

Swagger UI is available at /swagger/index.html

* *town mentioned above only has 1 resident
