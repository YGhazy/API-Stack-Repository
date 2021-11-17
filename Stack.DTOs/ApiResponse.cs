using System;
using System.Collections.Generic;
using System.Text;
using Stack.DTOs.Enums;

namespace Stack.DTOs
{
    public class ApiResponse<TData>
    {
        public TData Data { get; set; }
        public bool Succeeded { get; set; } = false;
        public List<string> Errors { get; set; } = new List<string>();
        public ErrorType? ErrorType { get; set; }
    }
}
