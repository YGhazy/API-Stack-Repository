using System;
using System.Collections.Generic;

namespace Stack.Entities.Models
{
    public partial class ConnectionId
    {
        public string Id { get; set; }
    
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }


    }
}
