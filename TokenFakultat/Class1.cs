using System;
using System.Globalization;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;

namespace TokenFakultat
{
    public class Class1
    {
        //przykladowe generowanie tokena
        public string BuildToken(User user, string secret, string issure, DataTime expirationDate, string serverPath)
        {
            var key = new SymetricSecutiryKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.GivenName, user.UserName),
                new Claim(ClaimTypes.Role, "Admin")
            };

            //(strona ktora odbiera validacje, nasz front wysyla zapytanie, mozemy wogle nie podawać, kiedy token wygasa- mozemy nie podawac, uzyskujemy credentiale)
            var token = new JwtSecurityToken(serverPath, issuer, claims, expirationDate, SigningCredentials:credentials);

            //sprawdzamy czy w naglowku jest nasz token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    //moteoda w ajkims kontrolerze
    //mozemy przekazac role ze do tej metody ma tylko uprawnienai admin
    //Core ma Policy mozna sprawdzac czy ma role czy nie jesli nie to 403 Forbidden - strzelanie do bazy nie jest git 
    // lepiej cashowac role, zmieniac role przez cashe
    [Authorize(Policy="Admin")]
    public IActionResult GetUserInfo() {
        var username = User.Claims.FirstOrDefault(x => x.Type === ClaimTypes.GivenName);
    }
}
