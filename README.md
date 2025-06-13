## ShorterThanAverage - The Best URL Shortener in Town*!

<img src="https://i.postimg.cc/mgQjcG4h/Screenshot-2025-06-13-at-18-21-50.png" width="735" height="460">

#### Usage:
POST endpoint (For shortening):\
/api/shorten/

```json
Request body: application/json
{
	"url": "string",
	"vanity": "string"
}
```
*`vanity` is the short code that the user would like for their URL. If not provided, a generated short code will be used.*

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
