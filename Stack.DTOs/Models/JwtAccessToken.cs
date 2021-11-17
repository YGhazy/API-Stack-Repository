using System;
using System.Collections.Generic;
using System.Text;

namespace Stack.DTOs.Models
{
    // JWT Token Model . 
    public class JwtAccessToken
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }

    }

}
