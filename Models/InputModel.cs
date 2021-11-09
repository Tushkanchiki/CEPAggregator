using CEPAggregator.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CEPAggregator.Models
{
    public class InputModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Количество валюты должно быть положительным")]
        public int Count { get; set; }
        public CurrencyType Currency { get; set; }
        public bool UseLocation { get; set; }
        public bool UseRating { get; set; }
        public bool UseCurrency { get; set; }
    }
}
